// using Api_IA.Interfaces;
// using Api_IA.Models.Images;

// namespace Api_IA.Services
// {

//     public class TestService : ITest
//     {
//         private readonly IExecutionsRepository _executionsRepository;

//         public Testservice(IExecutionsRepository executionsRepository)
//         {
//             _executionsRepository = executionsRepository;
//         }
//         public async Task<List<bool>> CustomVisionAsyncTestMain(List<RequestImages> Images)
//         {

//             var tasks = new List<Task<bool>>();
//             int LastInsertedId = await _executionsRepository.AsyncTestPostmanInsertLogBD();
//             Console.WriteLine("Se creó la execución bien");

//             if (LastInsertedId != 0)
//             {
//                 foreach (var image in Images)
//                 {
//                     byte[] img = Convert.FromBase64String(image.Image_url);
//                     int img_width, img_height = 0;
//                     bool resize_flag = img.Length > 4194304;
//                     if (resize_flag)
//                     {
//                         (img_width, img_height) = await Task.Run(() =>
//                         {
//                             using var image = System.Drawing.Image.FromStream(new MemoryStream(img));
//                             return (image.Width, image.Height);
//                         });
//                     }

//                     string url = $"{Environment.GetEnvironmentVariable("BLOB_URL_BASE")}subidaendpoint/{image.Image_name}";//https://azbashstoragegerman.blob.core.windows.net/droneimages/subidaendpoint/testgerman2.jpg
//                     Console.WriteLine("Se hace el append al arreglo de tasks");
//                     var task = CustomVisionAsyncTestUploadImgToBlob(
//                         image.Image_name,   //testgerman2.jpg
//                         img,                //image.Image_url ya en base64
//                         url,                //url enpoint azure
//                         image.Format,       //formato imagen
//                         LastInsertedId,     //id
//                         resize_flag,        //bool
//                         img_height          //int tamaño
//                         );
//                     Console.WriteLine("Se hace el append al arreglo de tasks x2");

//                     tasks.Add(task);
//                 }
//                 Console.WriteLine("Antes de mostrar el result del tasks I guess");

//                 var results = await Task.WhenAll(tasks);
//                 return results.ToList();

//             }
//             return null;
//         }
//         public async Task<bool> CustomVisionAsyncTestUploadImgToBlob(string image_name, byte[] image, string url, string content_type, int last_inserted_id, bool resize_flag, int img_height)
//         {
//             try
//             {
//                 Console.WriteLine("Se hace la llamada del blob_status");
//                 int blob_status = await AsyncTestHttpUploadToBucket(image, url, content_type);
//                 Console.WriteLine("Se ejecutó el código de insertar el blob status");
//                 int insertedImageId = 0;
//                 if (blob_status == 201)
//                 {
//                     Console.WriteLine("El blob status fue 201");
//                     insertedImageId = await Task.Run(() => {
//                         return _executionsRepository.AsyncTestPostmanInsertImageBd(last_inserted_id, image_name, "subidaendpoint");
//                     });

//                     Console.WriteLine("Se insertó la imagen a la base de datos");
//                     if (insertedImageId != 0)
//                     {
//                         Console.WriteLine("Entramos al flujo de insertedImageId");
//                         string project_id = "1be6360f-e231-4f63-9821-e25dc7a1dfcf";
//                         string publishIterationName = "Iteration13";
//                         var client = new ApiCustomVision().GetPredictionClient();

//                         string temp_url = url;
//                         if (resize_flag)
//                             url = await new ApiKraken().AsyncResizeImage(temp_url, (int)(img_height * 0.8));

//                         var results = await client.DetectImageUrlAsync(Guid.Parse(project_id), publishIterationName, new ImageUrl(url));

//                         Console.WriteLine("No hubo error con el servicio de custom vision");
//                         Console.WriteLine("Se tratarán de sacar predicciones:");

//                         if (results != null)
//                         {
//                             Console.WriteLine("Sí hay predicciones");
//                             await _executionsRepository.CustomVisionAsyncTestInsertPredictionsToDb(results, image_name, last_inserted_id, url);
//                             Console.WriteLine("Se guardaron las predicciones en la base de datos");
//                             Console.WriteLine("Se hace update de la imagen");
//                             await _executionsRepository.UpdateImage(insertedImageId);
//                             return true;
//                         }
//                         Console.WriteLine("No hay predicciones");
//                         Console.WriteLine("Se hace update de la imagen");
//                         await _executionsRepository.UpdateImage(insertedImageId);
//                         return false;
//                     }
//                     Console.WriteLine("Error indexing image row to DB");
//                     return false;
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine("Error en CustomVisionAsyncTestUploadImgToBlob: " + ex);

//             }
//             Console.WriteLine("Error inserting Blob to Blob Storage");
//             return false;
//         }

//     }
// }
