using MrmTool.Common;
using NanoSVG;
using System.Runtime.Versioning;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml.Media;

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

    private static Color DecodeRGBA(uint encoded, float opacity = 1)
    {
        byte r = (byte)(encoded & 0xFF);
        byte g = (byte)((encoded >> 8) & 0xFF);
        byte b = (byte)((encoded >> 16) & 0xFF);
        byte a = (byte)((byte)((encoded >> 24) & 0xFF) * opacity);

        return new() { A = a, R = r, G = g, B = b };
    }

    [SupportedOSPlatform("windows10.0.18362")]
    public static unsafe CompositionBrush? CreateBrushFromNSVGPaint(this Compositor compositor, ref NSVGpaint paint, float opacity)
    {
        switch (paint.type)
        {
            case NSVGpaintType.NSVG_PAINT_COLOR:
                return compositor.CreateColorBrush(DecodeRGBA(paint.union.color, opacity));

            case NSVGpaintType.NSVG_PAINT_LINEAR_GRADIENT:
                {
                    var gradientBrush = compositor.CreateLinearGradientBrush();
                    for (int i = 0; i < paint.union.gradient->nstops; i++)
                    {
                        NSVGgradientStop stop = paint.union.gradient->Stops[i];
                        var color = DecodeRGBA(stop.color, opacity);
                        gradientBrush.ColorStops.Add(compositor.CreateColorGradientStop(stop.offset, color));
                    }

                    var xform = paint.union.gradient->xform;
                    gradientBrush.TransformMatrix = new(
                        xform[0], xform[1],
                        xform[2], xform[3],
                        xform[4], xform[5]);

                    gradientBrush.MappingMode = CompositionMappingMode.Absolute;
                    gradientBrush.StartPoint = new(0, 0);
                    gradientBrush.EndPoint = new(1, 0);
                    gradientBrush.ExtendMode = paint.union.gradient->spread switch
                    {
                        NSVGspreadType.NSVG_SPREAD_PAD => CompositionGradientExtendMode.Clamp,
                        NSVGspreadType.NSVG_SPREAD_REFLECT => CompositionGradientExtendMode.Mirror,
                        NSVGspreadType.NSVG_SPREAD_REPEAT => CompositionGradientExtendMode.Wrap,
                        _ => CompositionGradientExtendMode.Clamp
                    };

                    return gradientBrush;
                }

            case NSVGpaintType.NSVG_PAINT_RADIAL_GRADIENT:
                {
                    var gradientBrush = compositor.CreateRadialGradientBrush();
                    for (int i = 0; i < paint.union.gradient->nstops; i++)
                    {
                        NSVGgradientStop stop = paint.union.gradient->Stops[i];
                        var color = DecodeRGBA(stop.color, opacity);
                        gradientBrush.ColorStops.Add(compositor.CreateColorGradientStop(stop.offset, color));
                    }

                    var xform = paint.union.gradient->xform;
                    gradientBrush.TransformMatrix = new(
                        xform[0], xform[1],
                        xform[2], xform[3],
                        xform[4], xform[5]);

                    gradientBrush.MappingMode = CompositionMappingMode.Absolute;
                    gradientBrush.EllipseCenter = new(0, 0);
                    gradientBrush.EllipseRadius = new(1, 1);
                    gradientBrush.GradientOriginOffset = new(paint.union.gradient->fx, paint.union.gradient->fy);
                    gradientBrush.ExtendMode = paint.union.gradient->spread switch
                    {
                        NSVGspreadType.NSVG_SPREAD_PAD => CompositionGradientExtendMode.Clamp,
                        NSVGspreadType.NSVG_SPREAD_REFLECT => CompositionGradientExtendMode.Mirror,
                        NSVGspreadType.NSVG_SPREAD_REPEAT => CompositionGradientExtendMode.Wrap,
                        _ => CompositionGradientExtendMode.Clamp
                    };

                    return gradientBrush;
                }

            case NSVGpaintType.NSVG_PAINT_NONE:
                return null;

            default:
                ThrowHelpers.ThrowArgumentException("Unknown SVG paint type.");
                return null;
        }
    }

#if false
    public static unsafe Brush? CreateBrushFromNSVGPaint(ref NSVGpaint paint, float opacity)
    {
        switch (paint.type)
        {
            case NSVGpaintType.NSVG_PAINT_COLOR:
                return new SolidColorBrush(DecodeRGBA(paint.union.color, opacity));

            case NSVGpaintType.NSVG_PAINT_LINEAR_GRADIENT:
                {
                    var gradientBrush = new LinearGradientBrush();
                    for (int i = 0; i < paint.union.gradient->nstops; i++)
                    {
                        NSVGgradientStop stop = paint.union.gradient->Stops[i];
                        var color = DecodeRGBA(stop.color, opacity);
                        gradientBrush.GradientStops.Add(new() { Offset = stop.offset, Color = color });
                    }

                    var xform = paint.union.gradient->xform;
                    gradientBrush.Transform = new MatrixTransform() { Matrix = new(
                        xform[0], xform[1],
                        xform[2], xform[3],
                        xform[4], xform[5])};

                    gradientBrush.MappingMode = BrushMappingMode.Absolute;
                    gradientBrush.StartPoint = new(0, 0);
                    gradientBrush.EndPoint = new(1, 0);
                    gradientBrush.SpreadMethod= paint.union.gradient->spread switch
                    {
                        NSVGspreadType.NSVG_SPREAD_PAD => GradientSpreadMethod.Pad,
                        NSVGspreadType.NSVG_SPREAD_REFLECT => GradientSpreadMethod.Reflect,
                        NSVGspreadType.NSVG_SPREAD_REPEAT => GradientSpreadMethod.Repeat,
                        _ => GradientSpreadMethod.Pad
                    };

                    return gradientBrush;
                }

            case NSVGpaintType.NSVG_PAINT_RADIAL_GRADIENT:
                {
                    var gradientBrush = new RadialGradientBrush();
                    for (int i = 0; i < paint.union.gradient->nstops; i++)
                    {
                        NSVGgradientStop stop = paint.union.gradient->Stops[i];
                        var color = DecodeRGBA(stop.color, opacity);
                        gradientBrush.ColorStops.Add(compositor.CreateColorGradientStop(stop.offset, color));
                    }

                    var xform = paint.union.gradient->xform;
                    gradientBrush.TransformMatrix = new(
                        xform[0], xform[1],
                        xform[2], xform[3],
                        xform[4], xform[5]);

                    gradientBrush.MappingMode = CompositionMappingMode.Absolute;
                    gradientBrush.EllipseCenter = new(0, 0);
                    gradientBrush.EllipseRadius = new(1, 1);
                    gradientBrush.GradientOriginOffset = new(paint.union.gradient->fx, paint.union.gradient->fy);
                    gradientBrush.ExtendMode = paint.union.gradient->spread switch
                    {
                        NSVGspreadType.NSVG_SPREAD_PAD => CompositionGradientExtendMode.Clamp,
                        NSVGspreadType.NSVG_SPREAD_REFLECT => CompositionGradientExtendMode.Mirror,
                        NSVGspreadType.NSVG_SPREAD_REPEAT => CompositionGradientExtendMode.Wrap,
                        _ => CompositionGradientExtendMode.Clamp
                    };

                    return gradientBrush;
                    return null;
                }

            case NSVGpaintType.NSVG_PAINT_NONE:
                return null;

            default:
                ThrowHelpers.ThrowArgumentException("Unknown SVG paint type.");
                return null;
        }
    }
#endif

    [SupportedOSPlatform("windows10.0.18362")]
    public static unsafe CompositionContainerShape CreateShapeFromNSVGImage(this Compositor compositor, NSVGimage* image)
    {
        CompositionContainerShape containerShape = compositor.CreateContainerShape();

        for (NSVGshape* shape = image->shapes; shape != null; shape = shape->next)
        {
            if ((shape->flags & NSVGflags.NSVG_FLAGS_VISIBLE) is not NSVGflags.NSVG_FLAGS_VISIBLE)
                continue;

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

            spriteShape.FillBrush = compositor.CreateBrushFromNSVGPaint(ref shape->fill, shape->opacity);
            CompositionStrokeCap cap = shape->strokeLineCap switch
            {
                NSVGlineCap.NSVG_CAP_BUTT => CompositionStrokeCap.Flat,
                NSVGlineCap.NSVG_CAP_ROUND => CompositionStrokeCap.Round,
                NSVGlineCap.NSVG_CAP_SQUARE => CompositionStrokeCap.Square,
                _ => ThrowHelpers.ThrowArgumentException<CompositionStrokeCap>("Invalid line cap value.")
            };
            CompositionStrokeLineJoin join = shape->strokeLineJoin switch
            {
                NSVGlineJoin.NSVG_JOIN_MITER => CompositionStrokeLineJoin.Miter,
                NSVGlineJoin.NSVG_JOIN_ROUND => CompositionStrokeLineJoin.Round,
                NSVGlineJoin.NSVG_JOIN_BEVEL => CompositionStrokeLineJoin.Bevel,
                _ => CompositionStrokeLineJoin.Miter
            };
            spriteShape.StrokeBrush = compositor.CreateBrushFromNSVGPaint(ref shape->stroke, shape->opacity);
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