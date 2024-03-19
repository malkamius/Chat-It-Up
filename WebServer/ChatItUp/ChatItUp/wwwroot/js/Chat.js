var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();
skipCount = 0;
const limit = 3;
let loading = false;
let selectedChannelId;
let selectedServerId;
let contextMenu;
// Receiving messages from the server
connection.on("ReceiveMessage", function (channelid, userid, message) {
    if (channelid == selectedChannelId)
        appendMessage(userid, message, true);
});

const audioContext = new (window.AudioContext || window.webkitAudioContext)();

let audioQueue = [];
let isPlaying = false;
let isPlayingInner = false;
let nextTime = 0; // When the next audio chunk should start playing

// Function to process and play queued audio data
async function processAudioQueue() {
    if (isPlaying || audioQueue.length === 0) return;
    isPlaying = true;
    if (audioContext.state === 'suspended') {
        audioContext.resume();
    }
    while (audioQueue.length > 0) {
        const audioChunkArrayBuffer = audioQueue.shift();
        try {
            await audioContext.decodeAudioData(audioChunkArrayBuffer, function (audioBuffer) {
                const source = audioContext.createBufferSource();
                source.buffer = audioBuffer;
                source.connect(audioContext.destination);
                // Schedule the source to start playing at `nextTime`
                const now = audioContext.currentTime;
                source.start(Math.max(nextTime, now));
                // Update `nextTime` to schedule the next chunk
                nextTime = Math.max(nextTime, now) + audioBuffer.duration;
                source.onended = () => {
                    // When the audio chunk finishes playing, try to process the next one
                    isPlaying = false;
                    //processAudioQueue();
                };
            }, (err) => console.error(err));

        } catch (error) {
            console.error("Error playing audio chunk:", error);
            isPlaying = false;
        }
    }
}

// Revised event listener to queue audio chunks
connection.on("ReceiveAudioChunk", async base64AudioChunk => {
    console.log("Received audio");
    console.log(base64AudioChunk.substr(0, 500));
    const audioChunkArrayBuffer = Uint8Array.from(atob(base64AudioChunk), c => c.charCodeAt(0)).buffer;
    if (audioChunkArrayBuffer.byteLength > 0) {
        audioQueue.push(audioChunkArrayBuffer);
        processAudioQueue(); // Try to process the queue
    }
});


// Function to toggle the connection status display
function toggleConnectionStatus(isConnected) {
    const statusDiv = document.getElementById('connectionStatus');
    if (isConnected) {
        statusDiv.classList.remove('visible');
        statusDiv.classList.add('hidden');
    } else {
        statusDiv.classList.remove('hidden');
        statusDiv.classList.add('visible');

        if (selectedChannelId) {
            selectChannel(selectedChannelId);
        }
    }
}

// Monitor the connection status
connection.onclose(async () => {
    toggleConnectionStatus(false);
    attemptReconnect();
});

async function attemptReconnect() {
    let isConnected = false;
    while (!isConnected) {
        try {
            await connection.start();
            console.log("Reconnected to the SignalR hub successfully.");
            isConnected = true;
            toggleConnectionStatus(true);
        } catch (err) {
            console.error("Reconnection attempt failed, retrying...", err);
            // Wait for 1 or 5 seconds before trying to reconnect
            await new Promise(resolve => setTimeout(resolve, 5000)); // Adjust the timeout as needed
        }
    }
}

// Initially attempt to connect
connection.start().then(function () {
    toggleConnectionStatus(true);
}).catch(function (err) {
    console.error("Failed to connect SignalR hub on start.", err.toString());
    toggleConnectionStatus(false);
    attemptReconnect();
});

let messagesDiv;

function appendMessage(user, message, prepend = false, scroll = true) {
    const msgDiv = document.createElement("div");
    msgDiv.textContent = user + ": " + message;
    if (prepend) {
        messagesDiv.prepend(msgDiv); // Add messages to the top
        if (scroll)
            messagesDiv.scrollTop = messagesDiv.firstChild.clientHeight;
    } else {
        messagesDiv.appendChild(msgDiv); // Add messages to the bottom
        if (scroll)
            messagesDiv.scrollTop = messagesDiv.lastChild.clientHeight;
    }
}

async function loadMessages(initial = false) {
    if (loading) return;
    loading = true;

    const response = await fetch(`/api/chat/GetRecentMessages?channelId=${selectedChannelId}&skip=${skipCount}`);
    if (response.ok) {
        const messages = await response.json();
        messages.forEach(msg => appendMessage(msg.user, msg.body, false, skipCount == 0));
        skipCount += messages.length;
        if (messages.length > 0) {
            // Adjust scroll position only if we're not loading the initial page of messages
            //if (!initial) {
            //messagesDiv.scrollTop = messagesDiv.firstChild.clientHeight;
            //}
        }
    } else {
        console.error('Failed to fetch messages');
    }

    loading = false;
}



function selectChannel(channelId) {
    selectedChannelId = channelId;
    skipCount = 0; // Reset skip count for new channel
    messagesDiv.innerHTML = ''; // Clear existing messages

    document.querySelectorAll('.channel').forEach(channel => {
        channel.classList.remove('selected-channel');
        if (channel.dataset.channelId === channelId) {
            channel.classList.add('selected-channel');
        }
    });
    skipCount = 0;

    loadMessages(true); // Load messages for the selected channel
}

async function fetchChannelsForServer(serverId) {
    channels = [];
    if (loading) return;
    loading = true;

    const response = await fetch(`/api/chat/GetChannels?serverId=${serverId}`);
    if (response.ok) {
        channels = await response.json();
        updateChannelList(channels);

    } else {
        console.error('Failed to fetch messages');
    }

    loading = false;
    if (channels.length > 0)
        selectChannel(selectedChannelId ? selectedChannelId : channels[0].id);
}

function updateChannelList(channels) {
    const channelListDiv = document.getElementById('channelList');
    channelListDiv.innerHTML = ''; // Clear existing channels

    channels.forEach(channel => {
        const channelDiv = document.createElement('div');
        channelDiv.className = 'channel';
        channelDiv.textContent = channel.name;
        channelDiv.setAttribute('data-channel-id', channel.id);
        channelDiv.onclick = function () { selectChannel(channel.id); };
        channelListDiv.appendChild(channelDiv);
    });


}

function selectServer(serverId) {
    if (selectedServerId != serverId) selectedChannelId = undefined;
    selectedServerId = serverId;
    document.querySelectorAll('.server').forEach(server => {
        server.classList.remove('selected-server');
        if (server.dataset.serverId === serverId) {
            server.classList.add('selected-server');
        }
    });

    // Simulate fetching channels for the selected server
    // Replace this part with an actual API call if necessary
    channels = fetchChannelsForServer(serverId);

}

async function fetchServers() {
    servers = [];
    if (loading) return;
    loading = true;

    const response = await fetch(`/api/chat/GetServers`);
    if (response.ok) {
        servers = await response.json();
        updateServerList(servers);

    } else {
        console.error('Failed to fetch servers');
    }

    loading = false;

    if (servers.length > 0)
        selectServer(selectedServerId ? selectedServerId : servers[0].id);
}

function updateServerList(servers) {
    const serverListDiv = document.getElementById('serverList');
    serverListDiv.innerHTML = ''; // Clear existing channels

    servers.forEach(server => {
        const serverDiv = document.createElement('div');
        serverDiv.className = 'server';
        //serverDiv.textContent = server.name;
        serverDiv.setAttribute('data-server-id', server.id);

        const image = document.createElement('img');
        image.src = server.imageUrl; // Assuming server object has an `imageUrl` property
        image.alt = server.name;
        image.style.width = '64px';
        image.style.height = '64px';
        image.style.borderRadius = '50%'; // Makes the image circular
        image.title = server.name; // Tooltip

        serverDiv.appendChild(image);

        serverDiv.onclick = function () { selectServer(server.id); };
        serverListDiv.appendChild(serverDiv);
    });

    const serverDiv = document.createElement('div');
    serverDiv.className = 'server';
    //serverDiv.textContent = server.name;
    serverDiv.setAttribute('data-server-id', '*');

    const image = document.createElement('img');
    image.src = "/image/PlusSign.png"; // Assuming server object has an `imageUrl` property
    image.alt = 'Create new server';
    image.style.width = '64px';
    image.style.height = '64px';
    image.style.borderRadius = '50%'; // Makes the image circular
    image.title = 'Create new server'; // Tooltip

    serverDiv.appendChild(image);

    serverDiv.onclick = function () { showModalCreateServer(); };// window.location.href = "/CreateServer" };

    serverListDiv.appendChild(serverDiv);

}

let mediaRecorder = null;
let audioChunks = [];
let skip = 0;
let thestream = null;
async function ToggleAudio() {
    // Assuming your checkbox has an ID of "AudioCheckbox"
    let isChecked = document.getElementById("AudioCheckbox").checked;

    if (isChecked) {
        // Start recording
        if (mediaRecorder === null || mediaRecorder.state === "inactive") {
            navigator.mediaDevices.getUserMedia({ audio: true })
                .then(stream => {
                    thestream = stream;
                    mediaRecorder = new MediaRecorder(stream, { mimeType: 'audio/webm; codecs=opus' });
                    mediaRecorder.ondataavailable = event => {
                        //audioChunks.push(event.data);
                        if (mediaRecorder.state === 'recording') {
                            // After collecting a chunk, send it immediately
                            sendAudioChunk(event.data);

                        }
                    };
                    mediaRecorder.start(1000); // Split audio into chunks every 1 second

                });
        }
    } else {
        // Stop recording and send audio
        if (mediaRecorder && mediaRecorder.state !== "inactive") {
            mediaRecorder.stop();
            mediaRecorder.onstop = async () => {

                // Reset mediaRecorder to allow a new recording session
                mediaRecorder.stream.getTracks().forEach(track => track.stop());
                mediaRecorder = null;
                console.log("Recording stopped and data sent");
            };
        }
    }
}

async function sendAudioChunk(chunk) {
    chunk.arrayBuffer().then(arrayBuffer => {
        const base64String = btoa(String.fromCharCode(...new Uint8Array(arrayBuffer)));
        console.log(base64String.substr(0, 500));

        // Assuming you have a SignalR connection setup as `connection`
        connection.invoke("SendAudioChunk", base64String);
    });
}

$(document).ready(function () {
    messagesDiv = document.getElementById('messages');
    document.body.addEventListener('click', function () {
        if (audioContext.state === 'suspended') {
            audioContext.resume();
        }
    });
    document.getElementById("AudioCheckbox").addEventListener("click", function (event) {
        ToggleAudio();
    });

    messagesDiv.addEventListener('scroll', () => {
        isScrolledToTop = messagesDiv.scrollTop === (messagesDiv.clientHeight - messagesDiv.scrollHeight);

        // Check if scrolled to the top
        if (isScrolledToTop && !loading) {
            loadMessages();
        }
    });

    fetchServers();
});