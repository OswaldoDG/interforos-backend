

using System.Drawing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using A = DocumentFormat.OpenXml.Drawing;
using Xdr = DocumentFormat.OpenXml.Drawing.Spreadsheet;
using promodel.modelo;
using promodel.modelo.castings;
using promodel.modelo.perfil;
using promodel.modelo.proyectos;
using System.Globalization;
using Google.Apis.Json;
using DocumentFormat.OpenXml.Bibliography;

namespace promodel.servicios.castings;

 public static class ExtensionesCastingServicios
{
    public static ContactoCasting aContactoCasting(this ContactoUsuario usuario,DateTime? UltimoAcceso)
    {
        return new ContactoCasting()
        {
            Confirmado = false,
            Email = usuario.Email.ToLower(),
            Rol = (TipoRolCliente)usuario.Rol,
            UltimoIngreso = UltimoAcceso,
            UsuarioId = usuario.Id,              
        };
    }

    public static CastingListElement aCastingListElement(this Casting casting,TipoRolCliente rol)
    {
        return new CastingListElement()
        {
            Id = casting.Id,
            Nombre = casting.Nombre,
            NombreCliente = casting.NombreCliente,
            FechaApertura = casting.FechaApertura,
            FechaCierre = casting.FechaCierre,
            AceptaAutoInscripcion = casting.AceptaAutoInscripcion,
            Activo = casting.Activo,
            AperturaAutomatica = casting.AperturaAutomatica,
            CierreAutomatico = casting.CierreAutomatico,
            Rol =rol
        };
    }
    public static CastingListElement aCastingListElement(this Casting casting)
    {  
        return new CastingListElement()
        {            
            Id = casting.Id,
            Nombre = casting.Nombre,
            NombreCliente = casting.NombreCliente,
            FechaApertura = casting.FechaApertura,
            FechaCierre = casting.FechaCierre,
            AceptaAutoInscripcion = casting.AceptaAutoInscripcion,
            Activo = casting.Activo,
            AperturaAutomatica = casting.AperturaAutomatica,
            CierreAutomatico = casting.CierreAutomatico,
        };
    }

    public static void CrearArchivo(this Casting casting, List<ReporteModelosDTO> listaArtistas)
    {
        using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create($"C:\\interforos\\interforos-backend\\Reporte{casting.Nombre}.xlsx", SpreadsheetDocumentType.Workbook))
        {
            WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();
            Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

            // Crear todas las hojas necesarias primero
            HashSet<string> categorias = new HashSet<string>();
            foreach (var artista in listaArtistas)
            {
                categorias.Add(artista.Categoria);
            }

            foreach (var categoria in categorias)
            {
                InsertarHoja(workbookPart, categoria);
            }

            foreach (var artista in listaArtistas)
            {
                InsertarArtista(workbookPart, artista);
            }

            foreach (var artista in listaArtistas)
            {
                InsertarImg(workbookPart, artista, listaArtistas.Count());
            }

            workbookPart.Workbook.Save(); // Guardar el documento después de insertar datos
        }
    }



    public static void InsertarHoja(WorkbookPart workbookPart, string nombreCategoria)
    {
        WorksheetPart newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        newWorksheetPart.Worksheet = new Worksheet(new SheetData());

        Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();

        string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);
        uint sheetId = (uint)sheets.ChildElements.Count + 1;

        string sheetName = nombreCategoria;

        Sheet sheet = new Sheet()
        {
            Id = relationshipId,
            SheetId = sheetId,
            Name = sheetName
        };

        sheets.Append(sheet);

        Worksheet worksheet = newWorksheetPart.Worksheet;
        SheetData sheetData = worksheet.GetFirstChild<SheetData>();

        //Columns columns = new Columns();
        //Column column = new Column() { Min = 1, Max = 6, Width = 20, CustomWidth = true };
        //columns.Append(column);
        //worksheet.Append(columns);

        // Insertar encabezados solo si es la primera fila
        if (sheetData.Elements<Row>().Count() == 0)
        {
            Row headerRow = new Row() { RowIndex = 1};
            headerRow.Append(
                ConstructCell("Foto Principal"),
                ConstructCell("Nombre Artístico"),
                ConstructCell("Nombre Persona"),
                ConstructCell("Género"),
                ConstructCell("Edad"),
                ConstructCell("Habilidades")
            );
            sheetData.Append(headerRow);
        }
    }

    public static void InsertarArtista(WorkbookPart workbookPart, ReporteModelosDTO artista)
    {
        WorksheetPart worksheetPart = GetWorksheetPartByName(workbookPart, artista.Categoria);

        if (worksheetPart == null)
        {
            // Si la hoja no existe, la creamos aquí
            InsertarHoja(workbookPart, artista.Categoria);
            worksheetPart = GetWorksheetPartByName(workbookPart, artista.Categoria);
        }

        if (worksheetPart != null)
        {
            SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
            uint rowIndex = (uint)sheetData.ChildElements.Count + 1;
            Row dataRow = new Row() { RowIndex = rowIndex };
            dataRow.Append(
                ConstructCell(""),
                ConstructCell(artista.NombreArtistico),
                ConstructCell(artista.NombrePersona),
                ConstructCell(artista.Genero),
                ConstructCell(artista.Edad.ToString()),
                ConstructCell(artista.Habilidades)
            );
            sheetData.Append(dataRow);


            worksheetPart.Worksheet.Save();
        }
    }

    public static void InsertarImg(WorkbookPart workbookPart, ReporteModelosDTO artista, int tamArtistList)
    {
        WorksheetPart worksheetPart = GetWorksheetPartByName(workbookPart, artista.Categoria);

        if(worksheetPart == null)
        {
            InsertarHoja(workbookPart, artista.Categoria);
            worksheetPart = GetWorksheetPartByName(workbookPart, artista.Categoria);
        }

        if(worksheetPart!= null)
        {
            var pos = 2;
            for(int i=pos; i<tamArtistList; i++)
            {
                Cell celda = worksheetPart.Worksheet.Descendants<Cell>().Where(c => c.CellReference == "B2").FirstOrDefault();
                //Aquí no encuentra valor en la celda
                var valor = celda.CellValue.InnerText;
                if (!string.IsNullOrEmpty(valor))
                {
                    InsertarImagenEnHojaExistente(workbookPart, artista.FotoPrincipal, artista.Categoria, 0, pos);
                }
            }

            worksheetPart.Worksheet.Save();
        }
    }


    private static Cell ConstructCell(string value)
    {
        Cell cell = new Cell();
        cell.DataType = CellValues.String;
        cell.CellValue = new CellValue(value);


        return cell;
    }

    private static WorksheetPart GetWorksheetPartByName(WorkbookPart workbookPart, string sheetName)
    {
        Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
        foreach (Sheet sheet in sheets.Elements<Sheet>())
        {
            if (sheet.Name == sheetName)
            {
                return (WorksheetPart)workbookPart.GetPartById(sheet.Id);
            }
        }
        return null;
    }

    public static void InsertarImagenEnHojaExistente(WorkbookPart workbookPart, string imageFileName, string categoria, int columnIndex, int rowIndex)
    {
                WorksheetPart worksheetPart = GetWorksheetPartByName(workbookPart, categoria);


                if (worksheetPart == null)
                {
                    // Si la hoja no existe, la creamos aquí
                    InsertarHoja(workbookPart, categoria);
                    worksheetPart = GetWorksheetPartByName(workbookPart, categoria);
                }

                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                DrawingsPart drawingsPart = worksheetPart.DrawingsPart ?? worksheetPart.AddNewPart<DrawingsPart>();

                // Corrección: Crear un nuevo WorksheetDrawing y asignar elementos
                drawingsPart.WorksheetDrawing = new Xdr.WorksheetDrawing();
                drawingsPart.WorksheetDrawing.Append(new Xdr.TwoCellAnchor());

                if (!worksheetPart.Worksheet.ChildElements.OfType<Drawing>().Any())
                {
                    worksheetPart.Worksheet.Append(new Drawing { Id = worksheetPart.GetIdOfPart(drawingsPart) });
                }

                // Insertar la imagen en la hoja seleccionada
                ImagePart imagePart = drawingsPart.AddImagePart(ImagePartType.Jpeg);

                using (var stream = new FileStream(imageFileName, FileMode.Open))
                {
                    imagePart.FeedData(stream);
                }

                Bitmap bm = new Bitmap(imageFileName);
                DocumentFormat.OpenXml.Drawing.Extents extents = new DocumentFormat.OpenXml.Drawing.Extents();
                var maxWidth = bm.Width * (long)((float)914400 / bm.HorizontalResolution);
                var maxHeight = bm.Height * (long)((float)914400 / bm.VerticalResolution);
                var ratioX = maxWidth / bm.Width - 1000;
                var ratioY = maxHeight / bm.Height;
                var actualHeight = (long)(bm.Height * ratioX);
                var actualWidth = (long)(bm.Width * ratioY);
                bm.Dispose();

                var colOffset = 0;
                var rowOffset = 0;

                var nvps = drawingsPart.WorksheetDrawing.Descendants<Xdr.NonVisualDrawingProperties>();
                var nvpId = nvps.Count() > 0 ?
                    (UInt32Value)drawingsPart.WorksheetDrawing.Descendants<Xdr.NonVisualDrawingProperties>().Max(p => p.Id.Value) + 1 :
                    1U;

                var oneCellAnchor = new Xdr.OneCellAnchor(
                    new Xdr.FromMarker
                    {
                        ColumnId = new Xdr.ColumnId(columnIndex.ToString()),
                        ColumnOffset = new Xdr.ColumnOffset(colOffset.ToString()),
                        RowId = new Xdr.RowId(rowIndex.ToString()),
                        RowOffset = new Xdr.RowOffset(rowOffset.ToString())
                    },
                    new Xdr.Extent { Cx = actualWidth, Cy = actualHeight },
                    new Xdr.Picture(
                        new Xdr.NonVisualPictureProperties(
                            new Xdr.NonVisualDrawingProperties { Id = nvpId, Name = "Picture " + nvpId, Description = imageFileName },
                            new Xdr.NonVisualPictureDrawingProperties(new A.PictureLocks { NoChangeAspect = true })
                        ),
                        new Xdr.BlipFill(
                            new A.Blip { Embed = drawingsPart.GetIdOfPart(imagePart), CompressionState = A.BlipCompressionValues.Print },
                            new A.Stretch(new A.FillRectangle())
                        ),
                        new Xdr.ShapeProperties(
                            new A.Transform2D(
                                new A.Offset { X = 0, Y = 0 },
                                new A.Extents { Cx = actualWidth, Cy = actualHeight }
                            ),
                            new A.PresetGeometry { Preset = A.ShapeTypeValues.Rectangle }
                        )
                    ),
                    new Xdr.ClientData()
                );

                drawingsPart.WorksheetDrawing.Append(oneCellAnchor);

        worksheetPart.Worksheet.Save();

    }

}
