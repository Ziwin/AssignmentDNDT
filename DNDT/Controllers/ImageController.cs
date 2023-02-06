using Microsoft.AspNetCore.Mvc;
using DNDT.Models;
using System.Collections.Specialized;
using System.Xml.Linq;

namespace DNDT.Controllers
{
    [ApiController]
    [Route("image")]

    public class ImageController : ControllerBase
    {
        private const string ImageDir = "./Images";
        private const string CommentDir = "./Comments";

        [HttpPost("upload")]
        public IActionResult UploadImage([FromForm] ImageModel file)
        {
            // Check to see if it's a valid input
            if (string.IsNullOrEmpty(file.Name) || string.IsNullOrEmpty(file.Comment) || file.Image == null)
            {
                return BadRequest();
            }

            // Check to see if the directory exists
            if (!Directory.Exists(ImageDir))
            {
                Directory.CreateDirectory(ImageDir);
            }

            if (!Directory.Exists(CommentDir))
            {
                Directory.CreateDirectory(CommentDir);
            }

            Guid g = Guid.NewGuid(); // uuid

            var imagePath = Path.Combine(ImageDir, g + "_" + file.Name + Path.GetExtension(file.Image.FileName));

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                file.Image.CopyTo(fileStream); // File stream to create image
            }

            var infoPath = Path.Combine(CommentDir, g + "_" + file.Name + ".txt");
            System.IO.File.WriteAllText(infoPath, file.Comment); // Make comment .txt file

            return StatusCode(201, new { message = "Image uploaded successfully! Please refer to the id to continue with the API!", id = g}); // return 201 status code with message and uuid
        }

        [HttpGet("{id}")]
        public IActionResult DownloadImage(string id)
        {

            // Check to see if it's a valid input
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            // Check to see if there's a valid image directory
            if (!Directory.Exists(ImageDir))
            {
                Directory.CreateDirectory(ImageDir);
            }

            string[] files = Directory.GetFiles(ImageDir);

            foreach (var file in files)
            {
                var temp = Path.GetFileName(file).Split('_'); // [id_name.png] -> [id, name.png]
                string id_file = temp[0]; // [id]

                if (string.Compare(id, id_file) == 0) // checks the id given by user to id in file
                {
                    var image = System.IO.File.OpenRead(file); // open image
                    return Ok(image); // return 200
                }
            }

            return NotFound(); // image doesn't exist

        }


        [HttpPatch("info/{id}")]
        public IActionResult ModifyComment(string id, [FromForm] string comment)
        {
            // Check to see if it's a valid input
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(comment))
            {
                return BadRequest();
            }

            // Check to see if the Comments Directory exists
            if (!Directory.Exists(CommentDir))
            {
                Directory.CreateDirectory(CommentDir);
            }

            string[] files = Directory.GetFiles(CommentDir);

            /* Loop through Comments Directory to find the comment file that matches with the id provided by the user
               If match, modify the comment with new user input
            */

            foreach (var file in files)
            {
                var name = Path.GetFileName(file).Split('_'); // [id_name.png] -> [id, name.png]
                string id_file = name[0]; // [id]

                if (string.Compare(id, id_file) == 0) // checks the id given by user to id in file
                {
                    var infoPath = Path.Combine(CommentDir, id_file + "_" + name[1]);
                    System.IO.File.WriteAllText(infoPath, comment); // write to the comment txt file
                    return Ok(new {message = "Modified Comment!"}); // return 200
                }
            }

            return NotFound(); // Comment doesn't exist

        }

        [HttpGet("info/{id}")]
        public IActionResult GetImageInfo(string id)
        {
            // Check to see if it's a valid input
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            // Check to see if the directory exists
            if (!Directory.Exists(ImageDir))
            {
                Directory.CreateDirectory(ImageDir);
            }

            if (!Directory.Exists(CommentDir))
            {
                Directory.CreateDirectory(CommentDir);
            }

            string[] files = Directory.GetFiles(ImageDir);

            /*
                Loop through Image file directory for image that matches with the id provided.
                If match, get the image info and find the image's comment in the Comments directory to get comments.
            */
            foreach (var file in files)
            {
                var temp = Path.GetFileName(file).Split('_'); // [id_name.png] -> [id, name.png]
               
                string id_file = temp[0]; // [id]

                if (string.Compare(id, id_file) == 0)
                {
                    string name = temp[1]; // [name.png]
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(ImageDir + "/" + id_file + "_" + name); // Get file info of a file from the Images Directory

                    name = name.Split('.')[0]; // [name.png] -> [name, png]
                    var infoPath = Path.Combine(CommentDir, id_file + "_" + name + ".txt"); // Get comments
                    var comment = System.IO.File.ReadAllText(infoPath);
          
                    
                    return Ok(new { comment, info = new { fileInfo.FullName, fileInfo.CreationTime, fileInfo.Length, fileInfo.IsReadOnly } }); // return 200 with file info and comments
                }
            }

            return NotFound(); // Image doesn't exist
        }

        [HttpGet]
        public IActionResult GetAllImages()
        {
            // Check to see if the Image Directory exists
            if (!Directory.Exists(ImageDir))
            {
                Directory.CreateDirectory(ImageDir);
                return NotFound();
            }
            String[] imageFiles = Directory.GetFiles(ImageDir);

            // If no images in the directory
            if (imageFiles.Length == 0)
            {
                return NotFound();
            }

            // Return every image with 200 status
            return Ok(imageFiles);
           

        }

        [HttpGet("info")]

        public IActionResult GetAllInfo()
        {

            // Check to see if the Image Directory exists
            if (!Directory.Exists(ImageDir))
            {
                Directory.CreateDirectory(ImageDir);
            }


            string[] files = Directory.GetFiles(ImageDir);

            List<ListDictionary> fileInfoList = new List<ListDictionary>();

            /* Loop through the Image Directory and Get all the Image Info of each Image.
             * Store it in a dictionary and append it to a list
            */
            foreach (var file in files)
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(ImageDir + "/" + Path.GetFileName(file));

                ListDictionary list = new ListDictionary();
                list.Add("FullName", fileInfo.FullName);
                list.Add("CreationTime", fileInfo.CreationTime);
                list.Add("Length", fileInfo.Length);
                list.Add("IsReadOnly", fileInfo.IsReadOnly);
                

                fileInfoList.Add(list);

            }

            // No Image Found
            if (fileInfoList.Count == 0)
            {
                return NotFound();
            }

            
            return Ok(fileInfoList); // return 200 status code with list of dictionaries

        }




    }

}
