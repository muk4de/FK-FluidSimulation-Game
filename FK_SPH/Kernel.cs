
using FK_CLI;
using System;

class Kernel
{
    double h;
    double h2;
    double alpha;
    // poly6ƒJ[ƒlƒ‹ŠÖ”
    public Kernel(double argH)
    {
        h = argH;
        h2 = argH * argH;
        alpha = 4.0 / (Math.PI * Math.Pow(h, 8));
    }

    public double KernelFunc(double r2)
    {
        if(r2 < h2)
        {
            return alpha * Math.Pow(h2 - r2, 3);
        }
        else
        {
            return 0.0;
        }
    }

    public fk_Vector GradientFunc(fk_Vector R)
    {
        var r2 = R.Dist2();
        if(r2 < h2)
        {
            var c = -6.0 * alpha * Math.Pow(h2 - r2, 2);
            return new fk_Vector(c * R.x, c * R.y);
        }
        else
        {
            return new fk_Vector(0.0, 0.0, 0.0);
        }
    }
}