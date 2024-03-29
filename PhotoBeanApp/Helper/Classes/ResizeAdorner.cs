using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFStickerDemo;

namespace PhotoBeanApp.Helper.Classes
{
    public class ResizeAdorner : Adorner
    {
        VisualCollection AdornerVisual;
        Thumb thumb2;
        Rectangle Rec;
        double ratioWidth;
        double ratioHeight;
        public ResizeAdorner(UIElement adornedElement, double ratioWidth, double ratioHeight) : base(adornedElement)
        {
            this.ratioWidth = ratioWidth;
            this.ratioHeight = ratioHeight;

            AdornerVisual = new VisualCollection(this);
            thumb2 = new Thumb() { Background = Brushes.Coral, Height = 10, Width = 10 };
            Rec = new Rectangle() { Stroke = Brushes.Coral, StrokeThickness = 2, StrokeDashArray = { 3, 2 } };

            thumb2.DragDelta += Thumb2_DragDelta;

            AdornerVisual.Add(Rec);
            AdornerVisual.Add(thumb2);
        }

        private void Thumb2_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Sticker sticker = AdornedElement as Sticker;

            Canvas canvas = VisualTreeHelper.GetParent(sticker) as Canvas;

            //normal dragging
            //if (sticker != null)
            //{
            //    sticker.Width = Math.Max(sticker.Width + e.HorizontalChange, 0);
            //    sticker.Height = Math.Max(sticker.Height + e.VerticalChange, 0);
            //}


            //ratio dragging

            if (sticker != null && canvas != null)
            {
                double widthChange = e.HorizontalChange;
                double heightChange = e.VerticalChange;

                double canvasWidth = canvas.ActualWidth;
                double canvasHeight = canvas.ActualHeight;

                double stickerLeft = Canvas.GetLeft(sticker);
                double stickerTop = Canvas.GetTop(sticker);
                double stickerRight = stickerLeft + sticker.Width + widthChange;
                double stickerBottom = stickerTop + sticker.Height + heightChange;

                // Check if resizing would go out of bounds
                if (stickerLeft >= 0 && stickerRight <= canvasWidth &&
                    stickerTop >= 0 && stickerBottom <= canvasHeight)
                {
                    sticker.Width = Math.Max(sticker.Width + widthChange, 0);
                    sticker.Height = Math.Max(sticker.Height + heightChange, 0);
                }
            }


            //update temporary sticker size
            sticker.StickerInfo.Size = new System.Drawing.Size((int)(sticker.ActualWidth * ratioWidth), (int)(sticker.ActualHeight * ratioHeight));
        }

        protected override Visual GetVisualChild(int index)
        {
            return AdornerVisual[index];
        }

        protected override int VisualChildrenCount => AdornerVisual.Count;

        protected override Size ArrangeOverride(Size finalSize)
        {
            Rec.Arrange(new Rect(-2.5, -2.5, AdornedElement.DesiredSize.Width + 5, AdornedElement.DesiredSize.Height + 5));
            thumb2.Arrange(new Rect(AdornedElement.DesiredSize.Width - 5, AdornedElement.DesiredSize.Height - 5, 10, 10));

            return base.ArrangeOverride(finalSize);
        }
    }
}