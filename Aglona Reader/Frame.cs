using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AglonaReader
{

    public abstract class AbstractFrame : IDisposable
    {

        /// <summary>
        /// Denotes the side of the text where the frame is applied.
        /// 1 = second text; 2 = first text.
        /// </summary>
        public byte side;

        public bool visible;
        
        public int line1;
        public int line2;

        public int x1;
        public int x2;

        protected AbstractFrame(Collection<AbstractFrame> list)
        {
            if (list != null)
                list.Add(this);
        }


        public void FillByRenderInfo(RenderedTextInfo renderedTextInfo, byte side)
        {
            if (!renderedTextInfo.valid)
            {
                visible = false;
                return;
            }

            visible = true;

            this.side = side;

            line1 = renderedTextInfo.line1;
            line2 = renderedTextInfo.line2;

            x1 = renderedTextInfo.x1;
            x2 = renderedTextInfo.x2;
        }

        public abstract void Draw(ParallelTextControl pTC);

        public abstract void Dispose();
        
    }

    public class DoubleFrame
    {
        public AbstractFrame f1;
        public AbstractFrame f2;

        public DoubleFrame(Pen pen, Collection<AbstractFrame> list)
        {
            f1 = new Frame(pen, list);
            f1.side = 1;

            f2 = new Frame(pen, list);
            f2.side = 2;
        }

        public DoubleFrame(Brush brush, Collection<AbstractFrame> list)
        {
            f1 = new Background(brush, list);
            f1.side = 1;

            f2 = new Background(brush, list);
            f2.side = 2;
        }

        internal AbstractFrame Frame(byte side)
        {
            return side == 1 ? f1 : f2;
        }
    }


    public class Background : AbstractFrame
    {
        public Brush brush;
        
        public Background(Brush brush, Collection<AbstractFrame> list) : base(list)
        {
            this.brush = brush;
        }

        public sealed override void Dispose()
        {
            if (brush != null)
                brush.Dispose();
        }

        public override void Draw(ParallelTextControl pTC)
        {
            if (pTC != null)
                pTC.DrawBackground(this);
        }
    }



    public class Frame : AbstractFrame
    {

        public Pen pen;

        public static Pen CreatePen(Color _color, DashStyle _dashStyle, float _width)
        {
            Pen result = new Pen(_color, _width);
            result.StartCap = LineCap.Round;
            result.EndCap = LineCap.Round;
            result.DashStyle = _dashStyle;
            return result;
        }

        public Frame(Pen pen, Collection<AbstractFrame> list) : base(list)
        {
            visible = false;
            this.pen = pen;
            list.Add(this);
        }

        // Dispose() calls Dispose(true)
        public override void Dispose()
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
