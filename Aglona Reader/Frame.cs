using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public AbstractFrame(List<AbstractFrame> list)
        {
            list.Add(this);
        }


        public void FillByRenderInfo(RenderedTextInfo r, byte _side)
        {
            if (!r.valid)
            {
                visible = false;
                return;
            }

            visible = true;

            side = _side;

            line1 = r.line1;
            line2 = r.line2;

            x1 = r.x1;
            x2 = r.x2;
        }

        public abstract void Draw(ParallelTextControl pTC);

        public abstract void Dispose();
        
    }

    public class DoubleFrame
    {
        public AbstractFrame f1;
        public AbstractFrame f2;

        public DoubleFrame(Pen _p, List<AbstractFrame> _l)
        {
            f1 = new Frame(_p, _l);
            f1.side = 1;

            f2 = new Frame(_p, _l);
            f2.side = 2;
        }

        public DoubleFrame(Brush _b, List<AbstractFrame> _l)
        {
            f1 = new Background(_b, _l);
            f1.side = 1;

            f2 = new Background(_b, _l);
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
        
        public Background(Brush _b, List<AbstractFrame> _l) : base(_l)
        {
            brush = _b;
        }

        public override void Dispose()
        {
            if (brush != null)
                brush.Dispose();
        }

        public override void Draw(ParallelTextControl pTC)
        {
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

        public Frame(Pen _pen, List<AbstractFrame> list) : base(list)
        {
            visible = false;
            pen = _pen;
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
