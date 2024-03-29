using FrameLib.Utils;
using static FrameLib.Constant.PhotoParemeters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameLib.Frame
{
    public class Frames
    {
        private Frames(List<FrameType> list)
        {
            FrameTypes = list;
        }
        public List<FrameType> FrameTypes { get; }
        private static Frames instance = null;
        public static Frames Instance(List<FrameType> list)
        {
            if (instance == null) { instance = new Frames(list); }
            return instance;
        }
        public bool LoadTypeImage(string folderPath, string id)
        {
            FrameType type = FrameTypes.Where(x => x.Code.Equals(id)).FirstOrDefault();
            if (type != null)
            {
                type.BackgroundImages = Util.LoadImagesFromFolder(folderPath);
                return true;
            }
            else
            {
                return false;
            }
        }
        public FrameType GetType(string id)
        {
            return FrameTypes.Where(x => x.Code.Equals(id)).FirstOrDefault();
        }


    }
}
