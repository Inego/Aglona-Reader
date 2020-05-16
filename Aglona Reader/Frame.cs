using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AglonaReader
{

    public abstract class AbstractFrame
    {

        public Pen framePen;

        /// <summary>
        /// Denotes the newSide of the text where the frame is applied.
        /// 1 = second text; 2 = first text.
        /// </summary>
        public byte Side { get; set; }

        public bool Visible { get; set; }

        public int Line1 { get; set; }
        public int Line2 { get; set; }

        public int X1 { get; set; }
        public int X2 { get; set; }

        protected AbstractFrame(Collection<AbstractFrame> list)
        {
            list?.Add(this);
        }

        public void FillByRenderInfo(RenderedTextInfo renderedTextInfo, byte newSide)
        {
            if (renderedTextInfo == null
                || !renderedTextInfo.Valid)
            {
                Visible = false;
                return;
            }

            Visible = true;

            Side = newSide;

            Line1 = renderedTextInfo.Line1;
            Line2 = renderedTextInfo.Line2;

            X1 = renderedTextInfo.X1;
            X2 = renderedTextInfo.X2;
        }

        public abstract void Draw(ParallelTextControl parallelTextControl);

    }

    public class DoubleFrame
    {
        public AbstractFrame F1 { get; set; }
        public AbstractFrame F2 { get; set; }

        public DoubleFrame(Pen pen, Collection<AbstractFrame> list)
        {
            F1 = new Frame(pen, list) {Side = 1};

            F2 = new Frame(pen, list) {Side = 2};
        }

        public DoubleFrame(Brush brush, Collection<AbstractFrame> list)
        {
            F1 = new Background(brush, list) {Side = 1};
            F2 = new Background(brush, list) {Side = 2};
        }

        public void SetPen(Pen pen)
        {
            F1.framePen = pen;
            F2.framePen = pen;
        }


        internal AbstractFrame Frame(byte side)
        {
            return side == 1 ? F1 : F2;
        }

        public void SetVisibility(bool visibility)
        {
            F1.Visible = visibility;
            F2.Visible = visibility;
        }

    }


    public class Background : AbstractFrame, IDisposable
    {
        public Brush BackgroundBrush { get; set; }
        
        public Background(Brush brush, Collection<AbstractFrame> list) : base(list)
        {
            BackgroundBrush = brush;
        }

        public void Dispose()
        {
            BackgroundBrush?.Dispose();
        }

        public override void Draw(ParallelTextControl parallelTextControl)
        {
            parallelTextControl?.DrawBackground(this);
        }
    }



    public class Frame : AbstractFrame, IDisposable
    {

        public static Pen CreatePen(Color color, DashStyle dashStyle, float width)
        {
            var result = new Pen(color, width)
            {
                StartCap = LineCap.Round, EndCap = LineCap.Round, DashStyle = dashStyle
            };
            return result;
        }

        public Frame(Pen pen, Collection<AbstractFrame> list) : base(list)
        {
            Visible = false;
            if (pen != null)
                framePen = pen;
            list?.Add(this);
        }

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        // NOTE: Leave out the finalizer altogether if this class doesn't 
        // own unmanaged resources itself, but leave the other methods
        // exactly as they are. 
        ~Frame()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }
        
        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            if (framePen == null) return;
            
            // free managed resources
            framePen.Dispose();
            framePen = null;
        }

        public override void Draw(ParallelTextControl parallelTextControl)
        {
            parallelTextControl?.DrawFrame(this);
        }

    }
}
