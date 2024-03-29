using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using FrameLib6.Frame;
using FrameLib6.Render;
using FrameLib6.Utils;

class Program
{
    
    static void Main()
    {
        var list = new List<Image>();


        Image image = Image.FromFile("C:/Users/Admin/Downloads/anh1.png");
        Image image2 = Image.FromFile("C:/Users/Admin/Downloads/anh1.png");
        Image image3 = Image.FromFile("C:/Users/Admin/Downloads/anh1.png");
        Image image4 = Image.FromFile("C:/Users/Admin/Downloads/anh1.png");
        list.Add(image);
        list.Add(image2);
        list.Add(image3);
        list.Add(image4);



        //Frames.Instance.LoadListType();
        //Frames.Instance.LoadTypeImage("C:/Users/Admin/Desktop/PNG/TYPE1", "4a");
        //Frames.Instance.LoadTypeImage("C:/Users/Admin/Desktop/PNG", "2a");
        //RenderManager.Render(Frames.Instance.GetType("1a"), "FALL_abc.png", list);

        //Bitmap r2 = RenderManager.FrameImage(Frames.Instance.GetType("1a"), result, "CUOI_abc.png");

        //RenderManager.FrameImage(Frames.Instance.GetType("1a"), result, "FALL_abc.png");
        //RenderManager.Render(Frames.Instance.GetType("1a"), "5x5 01.png", list);

        var list1 = ReadAndParseJsonFileWithSystemTextJson.UseFileOpenReadTextWithSystemTextJson("D:\\HOC\\Intern\\Workspace\\TestImage\\TestImage\\FrameType.json");
        
        var b = Frames.Instance(list1);
        b.LoadTypeImage("C:/Users/Admin/Desktop/PNG/TYPE1", "4a");
        Bitmap result = RenderManager.GhepHinh(b.GetType("4a"), list);
        result = RenderManager.GhepBackground(b.GetType("4a"), result, "CUOI_abc.png");
        result.Save("cc.png");    
    }

}


