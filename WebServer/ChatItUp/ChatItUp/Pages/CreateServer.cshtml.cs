using ChatItUp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Imaging;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ChatItUp.Pages
{
    [Authorize]
    public class CreateServerModel : PageModel
    {
        public string CurrentUrl => $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";

        [BindProperty]
        [Required(ErrorMessage = "Server name is required.")]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        [StringLength(512, ErrorMessage = "Description must be no longer than 512 characters.")]
        public string Description { get; set; }

        [BindProperty]
        public string? ImageUrl { get; set; } = null;

        [BindProperty]
        public IFormFile? UploadedImage { get; set; } = null;

        Context.CIUDataDbContext _context;
        private HtmlEncoder _htmlEncoder;

        public CreateServerModel(Context.CIUDataDbContext context, HtmlEncoder htmlEncoder)
        {
            _context = context;
            _htmlEncoder = htmlEncoder;
            Description = string.Empty;
        }

        public void OnGet()
        {
            Guid userId = Guid.Empty;
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
            if(_context.Servers.Count(s => s.ServerOwner == userId) >= 10)
            {
                ModelState.AddModelError(string.Empty, "You already own 10 or more servers and cannot create any more.");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Byte[]? serverImageBytes = null;
            Guid userId = Guid.Empty;
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

            if (_context.Servers.Count(s => s.ServerOwner == userId) >= 10)
            {
                ModelState.AddModelError(string.Empty, "You already own 10 or more servers and cannot create any more.");
                //return Page();
                return BadRequest("You already own 10 or more servers and cannot create any more.");
            }

            if (UploadedImage != null && UploadedImage.Length > 2097152) // 2 MB
            {
                ModelState.AddModelError("UploadedImage", "The image must be smaller than 2MB.");
                //return Page();
                return BadRequest("The image must be smaller than 2MB.");
            }
            else if (UploadedImage == null && !string.IsNullOrEmpty(ImageUrl))
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(ImageUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var imageBytes = await response.Content.ReadAsByteArrayAsync();

                        if (imageBytes.Length > 2097152) // 2 MB
                        {
                            ModelState.AddModelError(string.Empty, "The downloaded image must be smaller than 2MB.");
                            //return Page();
                            return BadRequest("The downloaded image must be smaller than 2MB.");
                        }

                        try
                        {
#if WINDOWS
                            using (var stream = new MemoryStream(imageBytes))
                            {
                                using (var image = System.Drawing.Image.FromStream(stream))
                                {
                                    if (image.Width > 1024 || image.Height > 1024)
                                    {
                                        ModelState.AddModelError(string.Empty, "The downloaded image must be less than 1024x1024.");
                                        //return Page();
                                        return BadRequest("The downloaded image must be less than 1024x1024.");
                                    }
                                    using (var newstream = new System.IO.MemoryStream())
                                    {
                                        image.Save(newstream, ImageFormat.Png);
                                        serverImageBytes = stream.ToArray();
                                    }
                                }

                            }
#else
                            throw new Exception("Image urls are not supported on the server platform.");
#endif

                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError(string.Empty, "The provided image url was in an unrecognized format.");
                            //return Page();
                            return BadRequest("The provided image url was in an unrecognized format.");
                        }


                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Failed to download the image from the provided URL.");
                        //return Page();
                        return BadRequest("Failed to download the image from the provided URL.");
                    }
                }
            }

            if (UploadedImage != null)
            {

                using (var stream = new System.IO.MemoryStream())
                {
                    await UploadedImage.CopyToAsync(stream);
                    try
                    {
#if WINDOWS
                        using(var newstream = new System.IO.MemoryStream())
                        using (var image = System.Drawing.Image.FromStream(stream))
                        {
                            if (image.Width > 1024 || image.Height > 1024)
                            {
                                ModelState.AddModelError(string.Empty, "The downloaded image must be less than 1024x1024.");
                                //return Page();
                                return BadRequest("The downloaded image must be less than 1024x1024.");
                            }
                            
                            image.Save(newstream, ImageFormat.Png);
                            serverImageBytes = stream.ToArray();
                        }
#else
                        throw new Exception("Image urls are not supported on the server platform.");
#endif
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, "The provided image was in an unrecognized format.");
                        //return Page();
                        return BadRequest("The provided image was in an unrecognized format.");
                    }
                }

            }

            if (!string.IsNullOrEmpty(Description) && Description.Length > 512)
            {
                ModelState.AddModelError("Description", "The provided description must be no more than 512 characters.");
                //return Page();
                return BadRequest("The provided description must be no more than 512 characters.");
            }
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "There was an error with the data submitted.");
                //return Page();
                return BadRequest("There was an error with the data submitted.");
            }
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Name.Trim())) {
                ModelState.AddModelError("Name", "Name is required.");
                //return Page();
                return BadRequest("Name is required.");
            }
            Name = _htmlEncoder.Encode(Name.Trim());

            if (!string.IsNullOrEmpty(Description) && !string.IsNullOrEmpty(Description.Trim())) {
                Description = _htmlEncoder.Encode(Description.Trim());
            }
            
            var ServerInfo = new Models.Server() { CreatedOn = DateTime.Now, Description = Description, Id = Guid.NewGuid(), Image = serverImageBytes, Name = Name, ServerOwner = userId };
            var UserServerInfo = new Models.UserServer() { JoinedOn = DateTime.Now, ServerId = ServerInfo.Id, UserId = userId };

            await _context.Servers.AddAsync(ServerInfo);
            await _context.UserServers.AddAsync(UserServerInfo);
            await _context.SaveChangesAsync();
            // Save the server details to the database or wherever necessary

            // Redirect to a confirmation page or the server list
            //return RedirectToPage("/Index");
            return new JsonResult(new {ServerId = ServerInfo.Id.ToString()});
        }
    }
}
