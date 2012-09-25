using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AglonaReader
{

    public abstract class AbstractFrame
    {

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
            if (list != null)
                list.Add(this);
        }


        public void FillByRenderInfo(RenderedTextInfo renderedTextInfo, byte newSide)
        {
            if (renderedTextInfo == null
                || !renderedTextInfo.valid)
            {
                this.Visible = false;
                return;
            }

            this.Visible = true;

            this.Side = newSide;

            this.Line1 = renderedTextInfo.line1;
            this.Line2 = renderedTextInfo.line2;

            this.X1 = renderedTextInfo.x1;
            this.X2 = renderedTextInfo.x2;
        }

        public abstract void Draw(ParallelTextControl parallelTextControl);

    }

    public class DoubleFrame
    {
        public AbstractFrame F1 { get; set; }
        public AbstractFrame F2 { get; set; }

        public DoubleFrame(Pen pen, Collection<AbstractFrame> list)
        {
            F1 = new Frame(pen, list);
            F1.Side = 1;

            F2 = new Frame(pen, list);
            F2.Side = 2;
        }

        public DoubleFrame(Brush brush, Collection<AbstractFrame> list)
        {
            this.F1 = new Background(brush, list);
            this.F1.Side = 1;

            this.F2 = new Background(brush, list);
            this.F2.Side = 2;
        }

        internal AbstractFrame Frame(byte side)
        {
            return side == 1 ? F1 : F2;
        }

        public void SetInvisible()
        {
            this.F1.Visible = false;
            this.F2.Visible = false;
        }

    }


    public class Background : AbstractFrame, IDisposable
    {
        public Brush BackgroundBrush { get; set; }
        
        public Background(Brush brush, Collection<AbstractFrame> list) : base(list)
        {
            this.BackgroundBrush = brush;
        }

        public void Dispose()
        {
            if (BackgroundBrush != null)
                BackgroundBrush.Dispose();
        }

        public override void Draw(ParallelTextControl parallelTextControl)
        {
            if (parallelTextControl != null)
                parallelTextControl.DrawBackground(this);
        }
    }



    public class Frame : AbstractFrame, IDisposable
    {

        public Pen pen;

        public static Pen CreatePen(Color color, DashStyle dashStyle, float width)
        {
            Pen result = new Pen(color, width);
            result.StartCap = LineCap.Round;
            result.EndCap = LineCap.Round;
            result.DashStyle = dashStyle;
            return result;
        }

        public Frame(Pen pen, Collection<AbstractFrame> list) : base(list)
        {
            Visible = false;
            if (pen != null)
                this.pen = pen;
            if (list != null)
                list.Add(this);
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
            if (disposing)
            {
                // free managed resources
                if (pen != null)
                {
                    pen.Dispose();
                    pen = null;
                }
            }

        }

        public override void Draw(ParallelTextControl pTC)
        {
            pTC.DrawFrame(this);
        }

        
        

    }
}
