using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        string folderPath = appPath + @"\";
        //string folderPath = "C:\\EasySheet";
        string spritesDirectory = folderPath + @"Sprites";
        if (!Directory.Exists(spritesDirectory))
        {
            Directory.CreateDirectory(spritesDirectory);
            Console.WriteLine("Directory successfully created: " + spritesDirectory);
        }
        string[] files = Directory.GetFiles(folderPath, "*.png");
        string imagePath = "0";
        string fileName;
        int inputH;
        string inputHRaw = "Не введено";
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[TURNING SHEET INTO SPRITES]");
        Console.ResetColor();
        Console.WriteLine("Leave field empty to skip");

        // Вызов первой функции - разделение изображения на спрайты
        if (files.Length > 0)
        {
            foreach (string file in files)
            {
                Image image = Image.FromFile(file);
                int imageWidth = image.Width;
                int imageHeight = image.Height;
                fileName = Path.GetFileName(file);
                string filePureName = Path.GetFileNameWithoutExtension(file);
                imagePath = folderPath + fileName;

                Console.Write("Current sheet: ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(fileName);
                Console.ResetColor();
                Console.Write("Amount of sprites by height: ");
                inputHRaw = Console.ReadLine();
                if (inputHRaw == "")
                {
                    Console.WriteLine("Skiping...");
                    Console.WriteLine("");
                    break;
                }
                else
                {
                    inputH = Convert.ToInt32(inputHRaw);
                }
                int spriteSizeH = imageHeight / inputH;
                Console.Write("Amount of sprites by width: ");
                int spriteSizeW = imageWidth / Convert.ToInt32(Console.ReadLine());
                SplitImageToSprites(imagePath, spriteSizeH, spriteSizeW, spritesDirectory, filePureName);
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("There is no sheets in app's directory.");
            Console.WriteLine("Put something to start turning!");
            Console.ResetColor();
        }

        // Вызов второй функции - создание изображения из спрайтов

        MergeSpritesToImage(spritesDirectory);

        Console.Write("PRESS ENTER TO QUIT...");
        Console.ReadLine();
    }

    static void SplitImageToSprites(string imagePath, int spriteSizeH, int spriteSizeW, string spritesDirectory, string filePureName)
    {
        using (Image image = Image.FromFile(imagePath))
        {
            int spriteCount = 0;
            for (int y = 0; y < image.Height; y += spriteSizeH)
            {
                for (int x = 0; x < image.Width; x += spriteSizeW)
                {
                    using (Bitmap sprite = new Bitmap(spriteSizeW, spriteSizeH))
                    {
                        using (Graphics graphics = Graphics.FromImage(sprite))
                        {
                            graphics.DrawImage(image, new Rectangle(0, 0, spriteSizeW, spriteSizeH),
                                new Rectangle(x, y, spriteSizeW, spriteSizeH), GraphicsUnit.Pixel);
                        }

                        string spriteFileName = Path.Combine(spritesDirectory, filePureName, filePureName + $"_sprite_{spriteCount:D2}.png");
                        if (!Directory.Exists(spritesDirectory + "\\" + filePureName))
                        {
                            Directory.CreateDirectory(spritesDirectory + "\\" + filePureName);
                        }
                        sprite.Save(spriteFileName, ImageFormat.Png);

                        spriteCount++;
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Sprites saved.");
            Console.ResetColor();
        }

    }

    static void MergeSpritesToImage(string spritesDirectory)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[TURNING SPRITES INTO SHEET]");
        Console.ResetColor();
        string[] spriteFiles = Directory.GetFiles(spritesDirectory, "*_sprite_*.png");
        if (spriteFiles.Length > 0)
        {
            //string[] spriteFiles = Directory.GetFiles(spritesDirectory, "*_sprite_*.png");
            string imagePath = spriteFiles[0];
            Image image = Image.FromFile(imagePath);
            int spriteSizeW = image.Width;
            int spriteSizeH = image.Height;

            int spritesPerRow = Math.Min(4, spriteFiles.Length);
            int spritesPerColumn = (int)Math.Ceiling((double)spriteFiles.Length / spritesPerRow);
            int mergedImageWidth = spritesPerRow * spriteSizeW;
            int mergedImageHeight = spritesPerColumn * spriteSizeH;

            using (Bitmap mergedImage = new Bitmap(mergedImageWidth, mergedImageHeight))
            {
                using (Graphics graphics = Graphics.FromImage(mergedImage))
                {
                    for (int i = 0; i < spriteFiles.Length; i++)
                    {
                        using (Image sprite = Image.FromFile(spriteFiles[i]))
                        {
                            int x = (i % spritesPerRow) * spriteSizeW;
                            int y = (i / spritesPerRow) * spriteSizeH;
                            graphics.DrawImage(sprite, new Point(x, y));
                        }
                    }
                }
                string mergedImageFileName = Path.Combine(spritesDirectory, "MergedSprite.png");
                mergedImage.Save(mergedImageFileName, ImageFormat.Png);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Sprites are detected and turned into a sheet:");
            Console.WriteLine(spritesDirectory + "\\MergedSprite.png");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("No sprites found.");
            Console.WriteLine("Put them in " + spritesDirectory);
            Console.ResetColor();
        }
    }
}

