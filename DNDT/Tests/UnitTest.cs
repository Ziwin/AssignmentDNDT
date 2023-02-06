
using Xunit;

namespace DNDT.Tests
{
    public class UnitTest
    {
        private const string ImageDir = "./Images";
        private const string CommentDir = "./Comments";
        [Fact]
        public void CheckImage()
        {

            if (!Directory.Exists(ImageDir))
            {
                Assert.Fail("No Image Directory");
            }

            

            string[] files = Directory.GetFiles(ImageDir);

            foreach (var file in files)
            {
                var temp = Path.GetFileName(file).Split('_');
                if (temp.Length != 2)
                {
                    Assert.Fail("Wrong image name");
                }
                
            }

            Assert.True(true, "Image Checks Ok");
        }

        [Fact]
        public void CheckComment()
        {
            if (!Directory.Exists(CommentDir))
            {
                Assert.Fail("No Comment Directory");
            }

            string[] files = Directory.GetFiles(CommentDir);

            foreach (var file in files)
            {
                var temp = Path.GetFileName(file).Split('_');
                if (temp.Length != 2)
                {
                    Assert.Fail("Wrong comment name");
                }
            }

            Assert.True(true, "Image Checks Ok");
        }

        
    }



}
