using System.Drawing;

namespace ChelindbankEatery;

public static class ImageExtensions
{
	public static Image Crop(this Image image, Rectangle selection)
	{
		Bitmap bmp = image as Bitmap;

		// Check if it is a bitmap:
		if (bmp == null)
			throw new ArgumentException("No valid bitmap");

		// Crop the image:
		Bitmap cropBmp = bmp.Clone(selection, bmp.PixelFormat);

		// Release the resources:
		image.Dispose();

		return cropBmp;
	}
}