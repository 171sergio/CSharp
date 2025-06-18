namespace OtimizacaoDesempenho;

public struct PixelRGB
{
    public byte R, G, B;

    public PixelRGB(byte r, byte g, byte b)
    {
        R = r; G = g; B = b;
    }

    public static PixelRGB Average(PixelRGB a, PixelRGB b, PixelRGB c, PixelRGB d)
    {
        return new PixelRGB(
            (byte)((a.R + b.R + c.R + d.R) / 4),
            (byte)((a.G + b.G + c.G + d.G) / 4),
            (byte)((a.B + b.B + c.B + d.B) / 4)
        );
    }
} 