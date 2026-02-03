using NanoSVG;
using System.Runtime.Versioning;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using Windows.UI;
using Windows.UI.Composition;

namespace MrmTool.SVG;

internal static class GeometryExtensions
{
    private static unsafe void AddNSVGShape(this ref ID2D1GeometrySink commandSink, NSVGshape* shape)
    {
        commandSink.SetFillMode(shape->fillRule switch
        {
            NSVGfillRule.NSVG_FILLRULE_EVENODD => D2D1_FILL_MODE.D2D1_FILL_MODE_ALTERNATE,
            NSVGfillRule.NSVG_FILLRULE_NONZERO => D2D1_FILL_MODE.D2D1_FILL_MODE_WINDING,
            _ => D2D1_FILL_MODE.D2D1_FILL_MODE_ALTERNATE
        });

        for (NSVGpath* path = shape->paths; path != null; path = path->next)
        {
            commandSink.BeginFigure(new(path->pts[0], path->pts[1]), D2D1_FIGURE_BEGIN.D2D1_FIGURE_BEGIN_FILLED);
            for (int i = 0; i < path->npts - 1; i += 3)
            {
                float* p = &path->pts[i * 2];

                D2D1_BEZIER_SEGMENT segment;
                segment.point1.x = p[2];
                segment.point1.y = p[3];
                segment.point2.x = p[4];
                segment.point2.y = p[5];
                segment.point3.x = p[6];
                segment.point3.y = p[7];

                commandSink.AddBezier(&segment);
            }
            commandSink.EndFigure(path->closed ? D2D1_FIGURE_END.D2D1_FIGURE_END_CLOSED : D2D1_FIGURE_END.D2D1_FIGURE_END_OPEN);
        }
    }

    private static Color DecodeRGBA(uint encoded)
    {
        byte r = (byte)(encoded & 0xFF);
        byte g = (byte)((encoded >> 8) & 0xFF);
        byte b = (byte)((encoded >> 16) & 0xFF);
        byte a = (byte)((encoded >> 24) & 0xFF);
        return Color.FromArgb(a, r, g, b);
    }

    private static unsafe void xformInverse(Span<float> inv, float* t)
    {
        double invdet, det = (double)t[0] * t[3] - (double)t[2] * t[1];
        if (det > -1e-6 && det < 1e-6)
        {
            t[0] = 1.0f; t[1] = 0.0f;
            t[2] = 0.0f; t[3] = 1.0f;
            t[4] = 0.0f; t[5] = 0.0f;
            return;
        }
        invdet = 1.0 / det;
        inv[0] = (float)(t[3] * invdet);
        inv[2] = (float)(-t[2] * invdet);
        inv[4] = (float)(((double)t[2] * t[5] - (double)t[3] * t[4]) * invdet);
        inv[1] = (float)(-t[1] * invdet);
        inv[3] = (float)(t[0] * invdet);
        inv[5] = (float)(((double)t[1] * t[4] - (double)t[0] * t[5]) * invdet);
    }

    [SupportedOSPlatform("windows10.0.18362")]
    public static unsafe CompositionBrush? CreateBrushFromNSVGPaint(this Compositor compositor, ref NSVGpaint paint)
    {
        switch (paint.type)
        {
            case NSVGpaintType.NSVG_PAINT_COLOR:
                return compositor.CreateColorBrush(DecodeRGBA(paint.union.color));

            case NSVGpaintType.NSVG_PAINT_LINEAR_GRADIENT:
                {
                    var gradientBrush = compositor.CreateLinearGradientBrush();
                    for (int i = 0; i < paint.union.gradient->nstops; i++)
                    {
                        NSVGgradientStop stop = paint.union.gradient->Stops[i];
                        var color = DecodeRGBA(stop.color);
                        gradientBrush.ColorStops.Add(compositor.CreateColorGradientStop(stop.offset, color));
                    }
                    Span<float> inv = stackalloc float[6];
                    xformInverse(inv, paint.union.gradient->xform);

                    float sx = inv[4];
                    float sy = inv[5];
                    // (0,1)
                    float ex = inv[2] + inv[4];
                    float ey = inv[3] + inv[5];
                    gradientBrush.MappingMode = CompositionMappingMode.Absolute;
                    gradientBrush.StartPoint = new(sx, sy);
                    gradientBrush.EndPoint = new(ex, ey);
                    return gradientBrush;
                }

            case NSVGpaintType.NSVG_PAINT_RADIAL_GRADIENT:
                {
                    var gradientBrush = compositor.CreateRadialGradientBrush();
                    for (int i = 0; i < paint.union.gradient->nstops; i++)
                    {
                        NSVGgradientStop stop = paint.union.gradient->Stops[i];
                        var color = DecodeRGBA(stop.color);
                        gradientBrush.ColorStops.Add(compositor.CreateColorGradientStop(stop.offset, color));
                    }
                    Span<float> inv = stackalloc float[6];
                    xformInverse(inv, paint.union.gradient->xform);

                    float sx = inv[4];
                    float sy = inv[5];
                    // (0,1)
                    float ex = inv[2] + inv[4];
                    float ey = inv[3] + inv[5];
                    gradientBrush.MappingMode = CompositionMappingMode.Absolute;
                    gradientBrush.EllipseCenter = new(sx, sy);
                    gradientBrush.EllipseRadius = new(ex, ey);
                    return gradientBrush;
                }

            case NSVGpaintType.NSVG_PAINT_NONE:
                return null;

            default:
                throw new ArgumentException("Unknown SVG paint type.");
        }
    }

    [SupportedOSPlatform("windows10.0.18362")]
    public static unsafe CompositionContainerShape CreateShapeFromNSVGImage(this Compositor compositor, NSVGimage* image)
    {
        CompositionContainerShape containerShape = compositor.CreateContainerShape();

        for (NSVGshape* shape = image->shapes; shape != null; shape = shape->next)
        {
            using ComPtr<ID2D1PathGeometry> geometry = default;
            D2D1Factory.CreatePathGeometry(geometry.GetAddressOf());

            using (ComPtr<ID2D1GeometrySink> sink = default)
            {
                geometry.Get()->Open(sink.GetAddressOf());
                sink.Get()->AddNSVGShape(shape);
                sink.Get()->Close();
            }

            StaticGeometrySource2D gs = new((ID2D1Geometry*)geometry.Get());
            CompositionPath path = new(gs);
            CompositionPathGeometry pathGeo = compositor.CreatePathGeometry(path);
            CompositionSpriteShape spriteShape = compositor.CreateSpriteShape(pathGeo);

            spriteShape.FillBrush = compositor.CreateBrushFromNSVGPaint(ref shape->fill);
            CompositionStrokeCap cap = shape->strokeLineCap switch
            {
                NSVGlineCap.NSVG_CAP_BUTT => CompositionStrokeCap.Flat,
                NSVGlineCap.NSVG_CAP_ROUND => CompositionStrokeCap.Round,
                NSVGlineCap.NSVG_CAP_SQUARE => CompositionStrokeCap.Square,
                _ => throw new ArgumentException("Invalid line cap value.")
            };
            CompositionStrokeLineJoin join = shape->strokeLineJoin switch
            {
                NSVGlineJoin.NSVG_JOIN_MITER => CompositionStrokeLineJoin.Miter,
                NSVGlineJoin.NSVG_JOIN_ROUND => CompositionStrokeLineJoin.Round,
                NSVGlineJoin.NSVG_JOIN_BEVEL => CompositionStrokeLineJoin.Bevel,
                _ => CompositionStrokeLineJoin.Miter
            };
            spriteShape.StrokeBrush = compositor.CreateBrushFromNSVGPaint(ref shape->stroke);
            spriteShape.StrokeThickness = shape->strokeWidth;
            spriteShape.StrokeStartCap = cap;
            spriteShape.StrokeEndCap = cap;
            spriteShape.StrokeDashCap = cap;
            spriteShape.StrokeLineJoin = join;
            spriteShape.StrokeMiterLimit = shape->miterLimit;
            spriteShape.StrokeDashOffset = shape->strokeDashOffset;
            for (int dai = 0; dai < shape->strokeDashCount; dai++)
            {
                spriteShape.StrokeDashArray.Add(shape->strokeDashArray[dai]);
            }

            containerShape.Shapes.Add(spriteShape);
        }

        return containerShape;
    }
}