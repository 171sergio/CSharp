using System.Buffers;
using System.Diagnostics;

namespace OtimizacaoDesempenho;

public class ImageProcessorOptimized
{
    private const int IMAGE_WIDTH = 800;
    private const int IMAGE_HEIGHT = 600;
    private const int TOTAL_IMAGES = 500;
    private const int BufferSize = IMAGE_WIDTH * IMAGE_HEIGHT;

    public static void ProcessImages()
    {
        Console.WriteLine("\nIniciando processamento de imagens (versão otimizada com ArrayPool)...");
        var stopwatch = Stopwatch.StartNew();
        int processedCount = 0;
        
        var arrayPool = ArrayPool<PixelRGB>.Shared;

        for (int imageIndex = 0; imageIndex < TOTAL_IMAGES; imageIndex++)
        {
            var originalImage = arrayPool.Rent(BufferSize);
            var blurredImage = arrayPool.Rent(BufferSize);

            try
            {
                GenerateSyntheticImage(originalImage, imageIndex);
                ApplyBlurFilter(originalImage, blurredImage);
                SaveImage(blurredImage, $"processed_{imageIndex}.jpg");
                processedCount++;

                if (imageIndex % 50 == 0 && imageIndex > 0)
                {
                    Console.WriteLine($"Processadas {imageIndex} imagens...");
                }
            }
            finally
            {
                arrayPool.Return(originalImage);
                arrayPool.Return(blurredImage);
            }
        }

        stopwatch.Stop();

        Console.WriteLine($"Processamento concluído!");
        Console.WriteLine($"Imagens processadas: {processedCount}");
        Console.WriteLine($"Tempo total: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Tempo médio por imagem: {stopwatch.ElapsedMilliseconds / (double)processedCount:F2} ms");
    }

    private static void GenerateSyntheticImage(PixelRGB[] image, int seed)
    {
        var random = new Random(seed);
        for (int i = 0; i < image.Length; i++)
        {
            image[i] = new PixelRGB(
                (byte)random.Next(256),
                (byte)random.Next(256),
                (byte)random.Next(256)
            );
        }
    }

    private static void ApplyBlurFilter(PixelRGB[] original, PixelRGB[] blurred)
    {
        // Aplicação de blur 2x2 simples usando array 1D
        for (int y = 0; y < IMAGE_HEIGHT - 1; y++)
        {
            for (int x = 0; x < IMAGE_WIDTH - 1; x++)
            {
                int currentIndex = y * IMAGE_WIDTH + x;
                int rightIndex = y * IMAGE_WIDTH + (x + 1);
                int bottomIndex = (y + 1) * IMAGE_WIDTH + x;
                int bottomRightIndex = (y + 1) * IMAGE_WIDTH + (x + 1);

                blurred[currentIndex] = PixelRGB.Average(
                    original[currentIndex],
                    original[rightIndex],
                    original[bottomIndex],
                    original[bottomRightIndex]
                );
            }
        }
    }

    private static void SaveImage(PixelRGB[] image, string filename)
    {
        // Simula salvamento
    }
} 