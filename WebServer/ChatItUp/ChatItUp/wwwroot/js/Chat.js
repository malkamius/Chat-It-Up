var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();
skipCount = 0;
const limit = 3;
let loading = false;
let selectedChannelId;
let selectedServerId;
let serverOwner = false;
let contextMenu;
// Receiving messages from the server
connection.on("ReceiveMessage", function (channelid, userid, message) {
    if (channelid == selectedChannelId) {
        appendMessage(userid, message, true);
        skipCount++;
    }
});

connection.on("UserConnected", function (serverId, userId, userDisplayName, userIsOwner) {
    updateUserStatus(serverId, userId, userDisplayName, userIsOwner, "Online");
});

connection.on("UserDisconnected", function (serverId, userId, userDisplayName, userIsOwner) {
    updateUserStatus(serverId, userId, userDisplayName, userIsOwner, "Offline");
});

connection.on("UserLeft", function (serverId, userId) {
    removeUser(serverId, userId);
});

connection.on("RemoveServer", function (serverId) {
    removeServer(serverId);
});

connection.on("ServerAdded", async function (serverId, serverName, isOwner) {
    selectedServerId = undefined;
    selectedChannelId = undefined;
    await fetchServers();
    selectedServerId = undefined;
    selectedChannelId = undefined;
    selectServer(serverId);
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

connection.on("ServerRemoved", async serverId => {
    await fetchServers();
});

connection.on("ChannelAdded", async (serverId, channelId) => {
    if (selectedServerId == serverId) {
        await fetchChannelsForServer(serverId, serverOwner);
    }
});

connection.on("ChannelRemoved", async (channelId, serverId) => {
    if (selectedServerId == serverId) {
        await fetchChannelsForServer(serverId, serverOwner);
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

async function fetchChannelsForServer(serverId, isServerOwner = false) {
    skipCount = 0; // Reset skip count for new channel
    messagesDiv.innerHTML = ''; // Clear existing messages
    channels = [];
    if (loading) return channels;
    loading = true;

    if (serverId != "Hub" && serverId != "CreateServer") {
        const response = await fetch(`/api/chat/GetChannels?serverId=${serverId}`);
        if (response.ok) {
            channels = await response.json();
            updateChannelList(channels, serverId, isServerOwner);

        } else {
            console.error('Failed to fetch messages');
        }
    } else {
        updateChannelList(channels, serverId, false);
    }

    loading = false;
    if (channels.length > 0)
        selectChannel(selectedChannelId ? selectedChannelId : channels[0].id);

    return channels;
}

function updateChannelList(channels, serverId, isServerOwner) {
    const channelListDiv = document.getElementById('channelList');
    channelListDiv.innerHTML = ''; // Clear existing channels

    channels.forEach(channel => {
        const channelDiv = document.createElement('div');
        channelDiv.className = 'channel';
        channelDiv.textContent = channel.name;
        channelDiv.setAttribute('data-channel-id', channel.id);
        channelDiv.setAttribute('data-server-id', serverId);
        channelDiv.setAttribute('data-server-is-owner', isServerOwner);
        channelDiv.onclick = function () { selectChannel(channel.id); };
        channelListDiv.appendChild(channelDiv);
        if (channel.id == selectedChannelId || selectedChannelId == undefined) { selectChannel(channel.id); }
    });

    if (isServerOwner) {
        const channelDiv = document.createElement('div');
        channelDiv.className = 'channel';
        //serverDiv.textContent = server.name;
        channelDiv.setAttribute('data-channel-id', 'CreateChannel');

        const image = document.createElement('img');
        image.src = "/image/PlusSign.png"; // Assuming server object has an `imageUrl` property
        image.alt = 'Add channel';
        image.style.width = '32px';
        image.style.height = '32px';
        image.style.borderRadius = '50%'; // Makes the image circular
        image.title = 'Add channel'; // Tooltip

        const textspan = document.createElement('span');
        textspan.style = "margin-left: 4px;";
        textspan.textContent = 'Add Channel';
        channelDiv.onclick = function () { showModalCreateChannel(serverId); };
        channelDiv.appendChild(image);
        channelDiv.appendChild(textspan);
        channelListDiv.appendChild(channelDiv);
    }
}

async function selectServer(serverId) {
    if (selectedServerId != serverId) selectedChannelId = undefined;
    selectedServerId = serverId;
    let isOwner = false;
    serverOwner = false;
    document.querySelectorAll('.server').forEach(server => {
        server.classList.remove('selected-server');
        if (server.dataset.serverId === serverId) {
            server.classList.add('selected-server');
            if (server.dataset.serverIsOwner == "true") {
                isOwner = true;
                serverOwner = true;
            }
        }

    });

    try {
        fetch(`/api/GetServerInviteCode/GetInvite?serverId=${serverId}`)
            .then(response => response.json())
            .then(data => {
                document.getElementById('serverInviteCode').textContent = `${window.location.protocol}//${window.location.host}` + "/UseInvite?InviteCode=" + data.inviteCode;
            })
            .catch(error => {
                console.error('Error fetching invite code:', error);
                document.getElementById('serverInviteCode').textContent = 'Error fetching invite code.';
            });
    } catch { }
    channels = await fetchChannelsForServer(serverId, isOwner);
    updateChannelList(channels, serverId, isOwner);

    fetchAndDisplayUsers(serverId);
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
        await selectServer(selectedServerId ? selectedServerId : servers[0].id);
}

function updateServerList(servers) {
    const serverListDiv = document.getElementById('serverList');
    serverListDiv.innerHTML = ''; // Clear existing channels

    let serverDiv = document.createElement('div');
    serverDiv.className = 'server';
    //serverDiv.textContent = server.name;
    serverDiv.setAttribute('data-server-id', 'Hub');

    let image = document.createElement('img');
    image.src = "/image/hub-image.png"; // Assuming server object has an `imageUrl` property
    image.alt = 'Create new server';
    image.style.width = '64px';
    image.style.height = '64px';
    image.style.borderRadius = '50%'; // Makes the image circular
    image.title = 'Hub'; // Tooltip

    serverDiv.appendChild(image);

    serverDiv.onclick = function () { };

    serverListDiv.appendChild(serverDiv);

    servers.forEach(server => {
        serverDiv = document.createElement('div');
        serverDiv.className = 'server';
        //serverDiv.textContent = server.name;
        serverDiv.setAttribute('data-server-id', server.id);
        serverDiv.setAttribute('data-server-is-owner', server.isOwner);

        image = document.createElement('img');
        image.src = server.imageUrl; // Assuming server object has an `imageUrl` property
        image.alt = server.name;
        image.style.width = '64px';
        image.style.height = '64px';
        image.style.borderRadius = '50%'; // Makes the image circular
        image.title = server.name; // Tooltip

        serverDiv.appendChild(image);

        serverDiv.onclick = async function () { await selectServer(server.id); };
        serverListDiv.appendChild(serverDiv);
    });

    serverDiv = document.createElement('div');
    serverDiv.className = 'server';
    //serverDiv.textContent = server.name;
    serverDiv.setAttribute('data-server-id', 'CreateServer');

    image = document.createElement('img');
    image.src = "/image/PlusSign.png"; // Assuming server object has an `imageUrl` property
    image.alt = 'Create new server';
    image.style.width = '64px';
    image.style.height = '64px';
    image.style.borderRadius = '50%'; // Makes the image circular
    image.title = 'Create new server'; // Tooltip

    serverDiv.appendChild(image);

    serverDiv.onclick = function () { showModalCreateServer(); };

    serverListDiv.appendChild(serverDiv);

}

async function fetchAndDisplayUsers(serverId) {
    const userListDiv = document.getElementById('userList');
    userListDiv.innerHTML = ''; // Clear the current list

    try {
        const response = await fetch(`/api/GetServerInfo/GetUsers?serverId=${serverId}`);
        if (!response.ok) {
            throw new Error('Network response was not ok.');
        }
        const users = await response.json();

        users.forEach(user => {
            updateUserStatus(serverId, user.id, user.displayName, user.isOwner, user.status)
        });
    } catch (error) {
        console.error('Error fetching users:', error);
        userListDiv.innerHTML = '<p>Error fetching users.</p>';
    }
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

function removeUser(serverId, userId) {
    if (serverId == selectedServerId) {
        const userListDiv = document.getElementById('userList');
        let userDiv = document.querySelector(`.user-item[data-user-id="${userId}"]`);
        userListDiv.removeChild(userDiv);
    }
}

function updateUserStatus(serverId, userId, userDisplayName, isOwner, userStatus) {
    if (serverId == selectedServerId) {
        const userListDiv = document.getElementById('userList');
        let userDiv = document.querySelector(`.user-item[data-user-id="${userId}"]`);

        // If the user is already in the list, update their status
        if (userDiv) {
            const statusImg = userDiv.querySelector('.status-icon');
            statusImg.src = userStatus === 'Online' ? '/image/online-image.png' : '/image/offline-image.png';
        } else {
            // If the user is not in the list and is connecting, add them
            userDiv = document.createElement('div');
            userDiv.classList.add('user-item');
            userDiv.setAttribute('data-user-id', userId); // Useful for identification

            // Assuming you have a way to fetch or already know the user's displayName
            const displayName = "User's Display Name"; // Placeholder, replace with actual value

            const statusImg = document.createElement('img');
            statusImg.src = userStatus === 'Online' ? '/image/online-image.png' : '/image/offline-image.png'; // Adjust paths as necessary
            statusImg.alt = userStatus;
            statusImg.classList.add('status-icon'); // Add class for styling if needed

            userDiv.appendChild(statusImg);

            const displayNameSpan = document.createElement('span');
            displayNameSpan.textContent = userDisplayName;

            userDiv.appendChild(statusImg);
            userDiv.appendChild(displayNameSpan);


            // Owner image
            if (isOwner) {
                const ownerImg = document.createElement('img');
                ownerImg.src = '/image/owner-image.png'; // Adjust path as necessary
                ownerImg.alt = 'Owner';
                ownerImg.classList.add('owner-icon'); // Add class for styling if needed

                userDiv.appendChild(ownerImg);
            }
            // Optionally, add the owner image if the user is an owner
            // This requires knowledge of whether the user is an owner at this point

            userListDiv.appendChild(userDiv);
        }
    }
}
function removeServer(serverId) {
    
    const serverListDiv = document.getElementById('serverList');
    let serverDiv = document.querySelector(`.server[data-server-id="${serverId}"]`);
    serverListDiv.removeChild(serverDiv);

    if (serverId == selectedServerId) {
        selectServer("Hub");
    }
    
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