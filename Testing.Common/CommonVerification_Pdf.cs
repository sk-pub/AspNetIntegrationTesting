using Docnet.Core;
using Docnet.Core.Converters;
using Docnet.Core.Models;
using Docnet.Core.Readers;
using ImageMagick;

namespace Testing.Common
{
    public static partial class CommonVerification
    {
        static ConversionResult Convert(Stream stream, IReadOnlyDictionary<string, object> settings)
        {
            var pageDimensions = new PageDimensions(2);

            using var reader = DocLib.Instance.GetDocReader(stream.ToBytes(), pageDimensions);

            return Convert(reader, settings);
        }

        static ConversionResult Convert(IDocReader document, IReadOnlyDictionary<string, object> settings)
        {
            var targets = GetStreams(document, settings).ToList();
            return new(null, targets);
        }

        static NaiveTransparencyRemover transparencyRemover = new();

        static IEnumerable<Target> GetStreams(IDocReader document, IReadOnlyDictionary<string, object> settings)
        {
            var pagesToInclude = document.GetPageCount();
            var preserveTransparency = true;

            for (var index = 0; index < pagesToInclude; index++)
            {
                using var reader = document.GetPageReader(index);

                var rawBytes = preserveTransparency ? reader.GetImage() : reader.GetImage(transparencyRemover);

                var width = reader.GetPageWidth();
                var height = reader.GetPageHeight();

                var magickImage = new MagickImage();
                magickImage.ReadPixels(rawBytes, new PixelReadSettings(width, height, StorageType.Char, "BGRA"));

                AddMask(settings, magickImage, index);
                var magickStream = new MemoryStream();
                magickImage.Write(magickStream, MagickFormat.Png);

                yield return new("png", magickStream, null);
            }
        }
    }
}
