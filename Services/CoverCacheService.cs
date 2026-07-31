using PackMeUp.Helpers;
using PackMeUp.Models.DTO;
using PackMeUp.Services.Interfaces;
using SkiaSharp;

namespace PackMeUp.Services
{
    public class CoverCacheService : ICoverCacheService
    {
        private const int CoverCacheVersion = 1;

        public string GetOrCreateCover(TripDTO trip, string packingSummary)
        {
            var path = GetCoverPath(trip);

            if (File.Exists(path))
                return path;

            var renderer = new TripCoverRenderer();
            var bitmap = renderer.Render(trip, packingSummary);

            SaveBitmap(bitmap, path);

            return path;
        }

        private static string GetCoverPath(TripDTO trip)
        {
            var destination = MakeSafeFileName(trip.Destination);

            var fileName =
                $"trip_cover_v{CoverCacheVersion}_" +
                $"{trip.LocalTripId}_" +
                $"{destination}_" +
                $"{trip.CoverTheme}_" +
                $"{trip.StartDate:yyyyMMdd}_" +
                $"{trip.EndDate:yyyyMMdd}.png";

            return Path.Combine(FileSystem.AppDataDirectory, fileName);
        }

        private static string MakeSafeFileName(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "unknown";

            var invalidChars = Path.GetInvalidFileNameChars();

            return new string(
                value
                    .Where(character => !invalidChars.Contains(character))
                    .ToArray())
                .Replace(' ', '_')
                .ToLowerInvariant();
        }

        private static void SaveBitmap(SKBitmap bitmap, string path)
        {
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 90);

            using var stream = File.OpenWrite(path);
            data.SaveTo(stream);
        }
    }
}
