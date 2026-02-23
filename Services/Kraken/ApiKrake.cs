using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ApiDrones.infrastructure
{
    public class ApiKraken
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<string> AsyncResizeImage(string url, int heigth)
        {
            try
            {
                var bytes = await _httpClient.GetByteArrayAsync(url);

                using var image = Image.Load(bytes);

                image.Mutate(x => x.Resize(0, heigth));

                using var ms = new MemoryStream();
                await image.SaveAsJpegAsync(ms);

                byte[] newBytes = ms.ToArray();

                var content = new ByteArrayContent(newBytes);

                await _httpClient.PutAsync(url, content);

                return url;
            }
            catch
            {
                return url;
            }
        }
    }
}