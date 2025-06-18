using System.Diagnostics;

namespace OtimizacaoDesempenho;

public class ImageProcessorTrivial
{
    private const int IMAGE_WIDTH = 800;
    private const int IMAGE_HEIGHT = 600;
    private const int TOTAL_IMAGES = 500;

    public static void ProcessImages()
    {
        Console.WriteLine("Iniciando processamento de imagens (versão trivial)...");

        var stopwatch = Stopwatch.StartNew();
        int processedCount = 0;

        for (int imageIndex = 0; imageIndex < TOTAL_IMAGES; imageIndex++)
        {
            // Gera uma imagem sintética
            PixelRGB[,] originalImage = GenerateSyntheticImage(imageIndex);

            // Aplica filtro blur (cria novo array a cada operação)
            PixelRGB[,] blurredImage = ApplyBlurFilter(originalImage);

            // Simula salvamento
            SaveImage(blurredImage, $"processed_{imageIndex}.jpg");
            processedCount++;

            if (imageIndex % 50 == 0 && imageIndex > 0)
            {
                Console.WriteLine($"Processadas {imageIndex} imagens...");
            }
        }

        stopwatch.Stop();

        Console.WriteLine($"Processamento concluído!");
        Console.WriteLine($"Imagens processadas: {processedCount}");
        Console.WriteLine($"Tempo total: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Tempo médio por imagem: {stopwatch.ElapsedMilliseconds / (double)processedCount:F2} ms");
    }

    private static PixelRGB[,] GenerateSyntheticImage(int seed)
    {
        var image = new PixelRGB[IMAGE_HEIGHT, IMAGE_WIDTH];
        var random = new Random(seed);

        for (int y = 0; y < IMAGE_HEIGHT; y++)
        {
            for (int x = 0; x < IMAGE_WIDTH; x++)
            {
                image[y, x] = new PixelRGB(
                    (byte)random.Next(256),
                    (byte)random.Next(256),
                    (byte)random.Next(256)
                );
            }
        }

        return image;
    }

    private static PixelRGB[,] ApplyBlurFilter(PixelRGB[,] original)
    {
        int height = original.GetLength(0);
        int width = original.GetLength(1);

        // PROBLEMA: Nova alocação a cada chamada!
        var blurred = new PixelRGB[height, width];

        // Aplicação de blur 2x2 simples
        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                blurred[y, x] = PixelRGB.Average(
                    original[y, x],
                    original[y, x + 1],
                    original[y + 1, x],
                    original[y + 1, x + 1]
                );
            }
        }

        return blurred;
    }

    private static void SaveImage(PixelRGB[,] image, string filename)
    {
        // Simula salvamento - na prática salvaria em disco
    }
} 