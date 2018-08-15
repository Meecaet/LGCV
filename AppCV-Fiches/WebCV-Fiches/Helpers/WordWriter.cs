using System;
using System.Collections.Generic;
using System.Linq;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Vml.Office;
using DAL_CV_Fiches.Models.Graph;
using System.IO;

namespace WebCV_Fiches.Helpers
{
    public class WordWriter
    {
        private Utilisateur utilisateur;
        private WordprocessingDocument document;

        private const string mauveCode = "7030A0";
        private const string grisCode = "969696";
        private const int spaceSimple = 240;
        private const string seperateur = ";;;";

        private RunProperties GetRunProperties(string fontName, string colorName, string size, bool bold, bool italic)
        {
            RunProperties runProperties = new RunProperties();
            runProperties.Append(new RunFonts { Ascii = fontName, HighAnsi = fontName }, new FontSize { Val = size }, new Color { Val = colorName });

            if (bold)
                runProperties.Append(new Bold());

            if (italic)
                runProperties.Append(new Italic());
            return runProperties;
        }

        private void AddSpacingToElement(OpenXmlCompositeElement element, int before, int after, int betweenLines)
        {
            SpacingBetweenLines spacing = new SpacingBetweenLines
            {
                Before = new StringValue(before.ToString()),
                After = new StringValue(after.ToString())
            };

            if (betweenLines > 0)
                spacing.Line = new StringValue(betweenLines.ToString());

            ParagraphProperties paragraphProperties = element.GetFirstChild<ParagraphProperties>();
            if (paragraphProperties == null)
            {
                paragraphProperties = new ParagraphProperties();
                paragraphProperties.Append(spacing);
                element.Append(paragraphProperties);
            }
            else
            {
                paragraphProperties.Append(spacing);
            }
        }

        private void AddCharacterSpacingToElement(OpenXmlCompositeElement element, int space)
        {
            Spacing spacing = new Spacing
            {
                Val = space
            };

            Run run = element.GetFirstChild<Run>();
            if (run == null)
            {
                run = new Run();
                element.Append(run);
            }

            RunProperties runProperties = run.GetFirstChild<RunProperties>();
            if (runProperties == null)
            {
                runProperties = new RunProperties();
                runProperties.Append(spacing);
                run.Append(runProperties);
            }
            else
            {
                runProperties.Append(spacing);
            }
        }

        private void AddIndentationToElement(OpenXmlCompositeElement element, int left, int right)
        {
            Indentation indentation = new Indentation
            {
                Right = new StringValue(right.ToString()),
                Left = new StringValue(left.ToString())
            };

            ParagraphProperties paragraphProperties = element.GetFirstChild<ParagraphProperties>();
            if (paragraphProperties == null)
            {
                paragraphProperties = new ParagraphProperties();
                paragraphProperties.Append(indentation);
                element.Append(paragraphProperties);
            }
            else
            {
                paragraphProperties.Append(indentation);
            }
        }

        private void AddAligmentToParagrah(Paragraph paragraph, ParagraphAligment aligment)
        {
            Justification justification = new Justification();

            switch (aligment)
            {
                case ParagraphAligment.Gauche:
                    justification.Val = new EnumValue<JustificationValues>(JustificationValues.Left);
                    break;
                case ParagraphAligment.Droite:
                    justification.Val = new EnumValue<JustificationValues>(JustificationValues.Right);
                    break;
                case ParagraphAligment.Justifie:
                    justification.Val = new EnumValue<JustificationValues>(JustificationValues.Both);
                    break;
                case ParagraphAligment.Centre:
                    justification.Val = new EnumValue<JustificationValues>(JustificationValues.Center);
                    break;
            }

            ParagraphProperties paragraphProperties = paragraph.GetFirstChild<ParagraphProperties>();
            if (paragraphProperties == null)
            {
                paragraphProperties = new ParagraphProperties();
                paragraphProperties.Append(justification);
                paragraph.Append(paragraphProperties);
            }
            else
            {
                paragraphProperties.Append(justification);
            }
        }

        private Style GetNewParagraphStyle(string styleid, string stylename)
        {
            Style style = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = styleid,
                CustomStyle = true
            };

            StyleName styleName = new StyleName() { Val = stylename };
            BasedOn basedOn = new BasedOn() { Val = "Normal" };
            NextParagraphStyle nextParagraphStyle = new NextParagraphStyle() { Val = "Normal" };
            style.Append(styleName);
            style.Append(basedOn);
            style.Append(nextParagraphStyle);

            return style;
        }

        private void AddSpacingToStyle(Style style, int before, int after, int betweenLines)
        {
            StyleParagraphProperties styleParagraphProperties = style.StyleParagraphProperties;

            if (styleParagraphProperties == null)
            {
                style.StyleParagraphProperties = new StyleParagraphProperties();
                styleParagraphProperties = style.StyleParagraphProperties;
            }

            SpacingBetweenLines spacing = new SpacingBetweenLines
            {
                Before = new StringValue(before.ToString()),
                After = new StringValue(after.ToString()),
                Line = new StringValue(betweenLines.ToString())
            };

            styleParagraphProperties.Append(spacing);
            styleParagraphProperties.Append(new ContextualSpacing { Val = false });
        }

        private Paragraph GetNewParagraph(string text, string fontName = "Arial", string fontColor = "Black", int fontSize = 20, bool isBold = false, 
            bool isItalic = false, int before = 0, int after = 0, int betweenLines = spaceSimple, int left = 0, int right = 0, 
            ParagraphAligment aligment = ParagraphAligment.Gauche)
        {
            Paragraph paragraph = new Paragraph();
            Run paragraphRun = new Run();

            AddSpacingToElement(paragraph, before, after, betweenLines);
            AddIndentationToElement(paragraph, left, right);
            paragraphRun.Append(GetRunProperties(fontName, fontColor, fontSize.ToString(), isBold, isItalic));
            paragraphRun.Append(new Text(text));
            paragraph.Append(paragraphRun);
            AddAligmentToParagrah(paragraph, aligment);
            return paragraph;
        }

        private Paragraph GetNewParagraph(string[] text, string styleId = "")
        {
            Paragraph paragraph = new Paragraph();

            if (!string.IsNullOrEmpty(styleId))
            {
                ParagraphProperties paragraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = styleId }
                };

                paragraph.Append(paragraphProperties);
            }

            for (int i = 0; i < text.Length; i++)
                paragraph.Append(new Run((new Text { Text = text[i] })));

            return paragraph;
        }

        private Paragraph GetNewParagraph(Run[] text, string styleId = "")
        {
            Paragraph paragraph = new Paragraph();

            if (!string.IsNullOrEmpty(styleId))
            {
                ParagraphProperties paragraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = styleId }
                };

                paragraph.Append(paragraphProperties);
            }

            for (int i = 0; i < text.Length; i++)
                paragraph.Append(text[i]);

            return paragraph;
        }

        private Paragraph GetNewBlankParagraph(int blankLines = 0)
        {
            Paragraph blankParagraph;
            Run breakRun;

            blankParagraph = new Paragraph();
            breakRun = new Run();

            breakRun.Append(GetNewLineBreaker());

            if (blankLines > 0)
            {
                for (int i = 0; i < blankLines; i++)
                    blankParagraph.Append(breakRun.CloneNode(true));
            }
            else
                blankParagraph.Append(breakRun);

            return blankParagraph;
        }

        private Break GetNewLineBreaker()
        {
            Break _break = new Break();
            return _break;
        }

        private Drawing DrawingManager(string relationshipId, string name, Int64Value cxVal, Int64Value cyVal, string impPosition)
        {
            string haPosition = impPosition;
            if (string.IsNullOrEmpty(haPosition))
            {
                haPosition = "left";
            }
            // Define the reference of the image.
            DW.Anchor anchor = new DW.Anchor();
            anchor.Append(new DW.SimplePosition() { X = 0L, Y = 0L });
            anchor.Append(
                new DW.HorizontalPosition(
                    new DW.HorizontalAlignment(haPosition)
                )
                {
                    RelativeFrom =
                      DW.HorizontalRelativePositionValues.Column
                }
            );
            anchor.Append(
                new DW.VerticalPosition(
                    new DW.PositionOffset("0")
                )
                {
                    RelativeFrom =
                    DW.VerticalRelativePositionValues.Paragraph
                }
            );
            anchor.Append(
                new DW.Extent()
                {
                    Cx = cxVal,
                    Cy = cyVal
                }
            );
            anchor.Append(
                new DW.EffectExtent()
                {
                    LeftEdge = 0L,
                    TopEdge = 0L,
                    RightEdge = 0L,
                    BottomEdge = 0L
                }
            );
            /*if (!string.IsNullOrEmpty(impPosition))
            {
                anchor.Append(new DW.WrapSquare() { WrapText = DW.WrapTextValues.BothSides });
            }
            else
            {
                anchor.Append(new DW.WrapTopBottom());
            }*/
            anchor.Append(new DW.WrapNone());
            anchor.Append(
                new DW.DocProperties()
                {
                    Id = (UInt32Value)1U,
                    Name = name
                }
            );
            anchor.Append(
                new DW.NonVisualGraphicFrameDrawingProperties(
                      new A.GraphicFrameLocks() { NoChangeAspect = true })
            );
            anchor.Append(
                new A.Graphic(
                      new A.GraphicData(
                        new PIC.Picture(

                          new PIC.NonVisualPictureProperties(
                            new PIC.NonVisualDrawingProperties()
                            {
                                Id = (UInt32Value)0U,
                                Name = name + ".jpg"
                            },
                            new PIC.NonVisualPictureDrawingProperties()),

                            new PIC.BlipFill(
                                new A.Blip(
                                    new A.BlipExtensionList(
                                        new A.BlipExtension()
                                        {
                                            Uri =
                                            "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                        })
                                )
                                {
                                    Embed = relationshipId,
                                    CompressionState =
                                    A.BlipCompressionValues.Print
                                },
                                new A.Stretch(
                                    new A.FillRectangle())),

                          new PIC.ShapeProperties(

                            new A.Transform2D(
                              new A.Offset() { X = 0L, Y = 0L },

                              new A.Extents()
                              {
                                  Cx = cxVal,
                                  Cy = cyVal
                              }),

                            new A.PresetGeometry(
                              new A.AdjustValueList()
                            )
                            { Preset = A.ShapeTypeValues.Rectangle }
                          )
                        )
                  )
                      { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
            );

            anchor.DistanceFromTop = (UInt32Value)0U;
            anchor.DistanceFromBottom = (UInt32Value)0U;
            anchor.DistanceFromLeft = (UInt32Value)114300U;
            anchor.DistanceFromRight = (UInt32Value)114300U;
            anchor.SimplePos = false;
            anchor.RelativeHeight = (UInt32Value)251658240U;
            anchor.BehindDoc = false;
            anchor.Locked = false;
            anchor.LayoutInCell = true;
            anchor.AllowOverlap = true;

            Drawing element = new Drawing();
            element.Append(anchor);

            return element;
        }

        private Drawing GetNewLogoDrawing(string relationshipId)
        {
            Drawing element = new Drawing(
                        new DW.Inline
                        (
                             new DW.Extent() { Cx = 525357L, Cy = 510963L },
                             new DW.EffectExtent()
                             {
                                 LeftEdge = 0L,
                                 TopEdge = 0L,
                                 RightEdge = 0L,
                                 BottomEdge = 0L
                             },
                             new DW.DocProperties()
                             {
                                 Id = (UInt32Value)1U,
                                 Name = "LogoPersonalise"
                             },
                             new DW.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks() { NoChangeAspect = true }),
                             new A.Graphic(
                                 new A.GraphicData(
                                     new PIC.Picture(
                                         new PIC.NonVisualPictureProperties(
                                             new PIC.NonVisualDrawingProperties()
                                             {
                                                 Id = (UInt32Value)0U,
                                                 Name = "LogoPersonaliseJPG.jpg"
                                             },
                                             new PIC.NonVisualPictureDrawingProperties()),
                                         new PIC.BlipFill(
                                             new A.Blip(
                                                 new A.BlipExtensionList(
                                                     new A.BlipExtension()
                                                     {
                                                         Uri =
                                                           "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                     })
                                             )
                                             {
                                                 Embed = relationshipId,
                                                 CompressionState = A.BlipCompressionValues.Print
                                             },
                                             new A.Rectangle
                                             {
                                                 Right = new StringValue("11810"),
                                                 Bottom = new StringValue("12440"),
                                                 Top = new StringValue("10551"),
                                                 Left = new StringValue("11496")
                                             },
                                             //new A.Stretch(new A.FillRectangle())),
                                             new A.Stretch()),
                                         new PIC.ShapeProperties(
                                             new A.Transform2D(
                                                 new A.Offset() { X = -91L, Y = -108L },
                                                 new A.Extents() { Cx = 766445L, Cy = 766445L }),
                                             new A.PresetGeometry(
                                                 new A.AdjustValueList()
                                             )
                                             { Preset = A.ShapeTypeValues.Rectangle }))
                                 )
                                 { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                         )
                        {
                            DistanceFromTop = (UInt32Value)0U,
                            DistanceFromBottom = (UInt32Value)0U,
                            DistanceFromLeft = (UInt32Value)0U,
                            DistanceFromRight = (UInt32Value)0U,
                            EditId = "50D07946"
                        });

            return element;
        }

        private Drawing GetLogoImage(string imagePath)
        {
            MainDocumentPart mainPart = document.MainDocumentPart;
            ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Png);

            using (FileStream stream = new FileStream(imagePath, FileMode.Open))
            {
                imagePart.FeedData(stream);
            }
            Drawing drawing = DrawingManager(mainPart.GetIdOfPart(imagePart), "logoPersonalise", 768029, 768029, string.Empty);
            return drawing;
        }

        private void ConfigurePage(Body docBody)
        {
            SectionProperties sectionProps = new SectionProperties();

            PageMargin pageMargins = new PageMargin
            {
                Left = 1134,
                Right = 850,
                Bottom = 1077,
                Top = 1077,
                Footer = 454,
                Header = 720,
                Gutter = 0
            };

            PageSize pageSize = new PageSize
            {
                Width = 12242,
                Height = 15842,
                Code = 1
            };

            sectionProps.Append(pageSize, pageMargins);
            docBody.Append(sectionProps);
        }

        private void AddBulletToStyles(int bulletId, int level, string bulletName)
        {

            NumberingDefinitionsPart numberingPart = document.MainDocumentPart.AddNewPart<NumberingDefinitionsPart>(bulletName);

            Numbering numElement = new Numbering(
                new AbstractNum(
                    new Level(
                        new NumberingFormat() { Val = NumberFormatValues.Bullet },
                        new LevelText() { Val = "" },
                        new LevelJustification { Val = new EnumValue<LevelJustificationValues>(LevelJustificationValues.Left) },
                        new ParagraphProperties(new Indentation { Left = "360", Hanging = "360" }),
                        new RunProperties(new RunFonts { Ascii = "Wingdings", HighAnsi = "Wingdings" }, new Color { Val = mauveCode }, new FontSize { Val = "22" })
                    )
                    { LevelIndex = level }
                )
                { AbstractNumberId = bulletId },
                new NumberingInstance(
                    new AbstractNumId() { Val = bulletId }
                )
                { NumberID = bulletId }
            );

            numElement.Save(numberingPart);

            Style bulletStyle = GetNewParagraphStyle(bulletName, bulletName);
            NumberingProperties numberingProperties = new NumberingProperties
            {
                NumberingLevelReference = new NumberingLevelReference { Val = new Int32Value(level) },
                NumberingId = new NumberingId { Val = new Int32Value(bulletId) }
            };

            bulletStyle.Append(numberingProperties);
            document.MainDocumentPart.StyleDefinitionsPart.Styles.Append(bulletStyle);
        }

        private void AddStyleToStylesPart(Style style)
        {
            document.MainDocumentPart.StyleDefinitionsPart.Styles.Append(style);
        }

        private Style CreateTitre1Style()
        {
            Style titre1Style = GetNewParagraphStyle("Titre1", "heading 1");

            ParagraphProperties paragraphProperties = new ParagraphProperties();
            paragraphProperties.Append(new KeepNext());
            paragraphProperties.Append(new ParagraphBorders
            {
                TopBorder = new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Dotted), Size = 4, Space = 1, Color = mauveCode },
                BottomBorder = new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Dotted), Size = 4, Space = 1, Color = mauveCode }
            });
            paragraphProperties.Append(new Tabs(new TabStop { Val = new EnumValue<TabStopValues>(TabStopValues.Left), Position = 1559 }));
            paragraphProperties.Append(new OutlineLevel { Val = 0 });

            RunProperties runProperties = new RunProperties();
            runProperties.Append(new RunFonts { Ascii = "Arial Gras", HighAnsi = "Arial Gras" });
            runProperties.Append(new Bold());
            runProperties.Append(new Caps());
            runProperties.Append(new Color { Val = mauveCode });
            runProperties.Append(new FontSize { Val = "26" });

            titre1Style.Append(paragraphProperties, runProperties);
            return titre1Style;
        }

        private Style CreateTitre2Style()
        {
            Style titre2Style = GetNewParagraphStyle("Titre2", "heading 2");

            ParagraphProperties paragraphProperties = new ParagraphProperties();
            paragraphProperties.Append(new KeepNext());
            paragraphProperties.Append(new ParagraphBorders
            {
                BottomBorder = new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4, Space = 1, Color = mauveCode }
            });
            paragraphProperties.Append(new SpacingBetweenLines { Before = "240", After = "120", Line = spaceSimple.ToString()});
            paragraphProperties.Append(new OutlineLevel { Val = 0 });

            RunProperties runProperties = new RunProperties();
            runProperties.Append(new RunFonts { Ascii = "Arial Gras", HighAnsi = "Arial Gras" });
            runProperties.Append(new Bold());
            runProperties.Append(new SmallCaps());
            runProperties.Append(new Color { Val = mauveCode });
            runProperties.Append(new FontSize { Val = "24" });

            titre2Style.Append(paragraphProperties, runProperties);
            return titre2Style;
        }

        private Style CreateFooterStyle()
        {
            Style footerStyle = GetNewParagraphStyle("Footer", "footer");

            ParagraphProperties paragraphProperties = new ParagraphProperties();
            paragraphProperties.Append(new SpacingBetweenLines { Before = "840", After = "0", Line = spaceSimple.ToString() });

            RunProperties runProperties = new RunProperties();
            runProperties.Append(new RunFonts { Ascii = "Arial", HighAnsi = "Arial" });
            runProperties.Append(new Color { Val = grisCode });
            runProperties.Append(new FontSize { Val = "16" });

            footerStyle.Append(paragraphProperties, runProperties);
            return footerStyle;
        }

        private void AddStylesPartToDoc()
        {
            StyleDefinitionsPart part = document.MainDocumentPart.StyleDefinitionsPart;
            if (part == null)
            {
                part = document.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
                Styles root = new Styles();
                root.Save(part);
            }
        }

        private void AddCellToTableRow(ref TableRow tableRow, string text)
        {
            TableCell tableCell = new TableCell();
            Paragraph paragraph = new Paragraph();
            Run run = new Run();

            run.Append(new Text(text));
            paragraph.Append(run);
            tableCell.Append(paragraph);

            tableRow.Append(tableCell);
        }

        private void AddCellToTableRow(ref TableRow tableRow, TableCell cell, TableCellProperties cellProperties)
        {
            cell.Append(cellProperties);
            tableRow.Append(cell);
        }

        private Paragraph GetTitre(string titre, string style = "Titre1", int espaceAvant = 240, int espaceApres = 120, int espaceLine = spaceSimple)
        {
            Paragraph titreParagraph;
            ParagraphProperties paragraphProperties;
            titreParagraph = new Paragraph();
            paragraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = style }
            };
            titreParagraph.Append(paragraphProperties);
            titreParagraph.Append(new Run(new Text(titre)));
            AddSpacingToElement(titreParagraph, espaceAvant, espaceApres, spaceSimple);
            return titreParagraph;
        }

        private Paragraph GetPuce(string texte, int espaceAvant = 0, int espaceApres = 0, int espaceLine = spaceSimple)
        {
            Paragraph puceParagraph;
            Run puceRun;

            puceParagraph = new Paragraph();
            puceRun = new Run();
            puceRun.Append(GetRunProperties("Arial", "Black", "20", false, false), new Text(texte));

            ParagraphProperties paragraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = "Puce1" },
                Justification = new Justification { Val = new EnumValue<JustificationValues>(JustificationValues.Left) }
            };

            puceParagraph.Append(paragraphProperties, puceRun);
            AddSpacingToElement(puceParagraph, espaceAvant, espaceApres, spaceSimple);
            return puceParagraph;
        }

        private TableCellProperties GetCellProperty(string backgroundColor, string width, TableRowAlignmentValues alignment, TableVerticalAlignmentValues verticalAligment)
        {
            TableCellProperties cellProperties = new TableCellProperties();
            cellProperties.Append(new Shading { Fill = backgroundColor });
            cellProperties.Append(new TableWidth { Width = width });
            cellProperties.Append(new TableJustification { Val = new EnumValue<TableRowAlignmentValues>(alignment) });
            cellProperties.Append(new TableCellVerticalAlignment { Val = new EnumValue<TableVerticalAlignmentValues>(verticalAligment) });

            return cellProperties;
        }

        private TableCell GetCell(Paragraph paragraph, string backgroundColor = "White", string borderColor = "Black", BorderValues borderValue = BorderValues.Single,
            TableRowAlignmentValues alignment = TableRowAlignmentValues.Left, TableVerticalAlignmentValues verticalAligment = TableVerticalAlignmentValues.Center,
            bool isTop =  false, bool isBottom = false)
        {
            TableCell tableCell = new TableCell();
            TableCellProperties cellProperties = new TableCellProperties();
            cellProperties.Append(new Shading { Fill = backgroundColor });
            cellProperties.Append(new TableJustification { Val = new EnumValue<TableRowAlignmentValues>(alignment) });
            cellProperties.Append(new TableCellVerticalAlignment { Val = new EnumValue<TableVerticalAlignmentValues>(verticalAligment) });
            if (isBottom)
            {
                cellProperties.Append(new TableCellBorders
                {
                    BottomBorder = new BottomBorder { Val = new EnumValue<BorderValues>(borderValue), Size = 1, Color = borderColor }
                });
            }
            if (isTop)
            {
                cellProperties.Append(new TableCellBorders
                {
                    TopBorder = new TopBorder { Val = new EnumValue<BorderValues>(borderValue), Size = 1, Color = mauveCode },
                });
            }
            tableCell.Append(paragraph);
            tableCell.Append(cellProperties);
            return tableCell;
        }

        private TableCell GetNewCell(Paragraph text, TableCellProperties cellProperties)
        {
            TableCell cell = new TableCell();
            cell.Append(cellProperties);
            cell.Append(text);

            return cell;
        }

        public WordWriter(WordprocessingDocument document, Utilisateur utilisateur)
        {
            this.document = document;
            this.utilisateur = utilisateur;
        }

        public void CreateDummyTest()
        {
            document.AddMainDocumentPart();
            document.MainDocumentPart.Document = new Document();

            Document doc = document.MainDocumentPart.Document;
            Body docBody = new Body();
            doc.Body = docBody;

            AddStylesPartToDoc();
            ConfigurePage(doc.Body);
            AddStyleToStylesPart(CreateTitre1Style());
            AddStyleToStylesPart(CreateTitre2Style());
            AddStyleToStylesPart(CreateFooterStyle());
            
            AddBulletToStyles(38, 0, "Puce1");
            ApplyFooter();

            CreateBioSection(doc.Body);
            CreateDomainesDInterventionSection(doc.Body);
            CreateFormationAcademiqueEtCertificationSection(doc.Body);
            CreateResumeDIntervention(doc.Body);
            CreateMandats(doc.Body);

            CreateTechnologies(doc.Body);
            CreatePerfectionnement(doc.Body);
            CreateAutresFormations(doc.Body);
            CreateAssociations(doc.Body);
            CreatePublications(doc.Body);
            CreateConferences(doc.Body);
            CreateLangues(doc.Body);

            string currentDate = DateTime.Now.ToString("Le dd MMMM yyyy");
            doc.Body.Append(new Paragraph(), new Paragraph(), new Paragraph(), new Paragraph());
            doc.Body.Append(GetNewParagraph(currentDate, aligment: ParagraphAligment.Droite));
            
            document.Save();
            document.Close();
        }

        private void CreateBioSection(Body docBody)
        {
            Table table = new Table();
            TableRow tableRow = new TableRow();
            TableCell imageCell = new TableCell();
            TableCell nomEtFonctionCell = new TableCell();
            TableCellProperties tableCellProperties;

            Paragraph nomEtFonctionParagraph;

            Style bioParagraphStyle = GetNewParagraphStyle("BioP1", "BioParagraph");
            AddSpacingToStyle(bioParagraphStyle, 240, 240, 0);

            document.MainDocumentPart.StyleDefinitionsPart.Styles.Append(bioParagraphStyle);

            TableProperties tableProperties = new TableProperties(
                new TableWidth
                {
                    Width = "10828",
                    Type = TableWidthUnitValues.Dxa
                },
                new TableIndentation
                {
                    Width = -459,
                    Type = TableWidthUnitValues.Dxa
                },
                new TableCellMarginDefault
                {
                    TableCellRightMargin = new TableCellRightMargin { Width = 108, Type = new EnumValue<TableWidthValues>(TableWidthValues.Dxa) },
                    TableCellLeftMargin = new TableCellLeftMargin { Width = 108, Type = new EnumValue<TableWidthValues>(TableWidthValues.Dxa) }
                });

            table.AppendChild<TableProperties>(tableProperties);

            tableCellProperties = new TableCellProperties();
            tableCellProperties.Append(new TableCellWidth { Width = new StringValue("1338"), Type = new EnumValue<TableWidthUnitValues>(TableWidthUnitValues.Dxa) });
            imageCell.Append(tableCellProperties);

            imageCell.Append(new Paragraph(new Run(GetLogoImage(@"images\logo.png"))));

            Run nomRun = new Run(), breakLine = new Run(), fonctionRun = new Run();
            nomRun.Append(GetRunProperties("Arial", mauveCode, "28", true, false));
            nomRun.Append(new Text { Text = $"{utilisateur.Prenom} {utilisateur.Nom}".Trim().ToUpper() });

            breakLine.Append(GetNewLineBreaker());

            fonctionRun.Append(GetRunProperties("Arial", "808080", "24", true, false));
            fonctionRun.Append(new Text { Text = utilisateur.Conseiller.Fonction.Description });

            nomEtFonctionParagraph = new Paragraph();
            AddSpacingToElement(nomEtFonctionParagraph, 240, 240, spaceSimple);
            AddIndentationToElement(nomEtFonctionParagraph, -40, 34);
            nomEtFonctionParagraph.Append(nomRun, breakLine, fonctionRun);

            nomEtFonctionCell.Append(nomEtFonctionParagraph);

            tableCellProperties = new TableCellProperties();
            tableCellProperties.Append(new TableCellBorders
            {
                TopBorder = new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1, Color = mauveCode },
                BottomBorder = new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1, Color = mauveCode }
            });
            nomEtFonctionCell.Append(tableCellProperties);

            tableRow.Append(imageCell, nomEtFonctionCell);
            table.Append(tableRow);
            docBody.Append(table);

            string[] phraseDiviser = utilisateur.Conseiller.CVs.First().ResumeExperience.Split(seperateur);

            foreach (string phrase in phraseDiviser)
            {
                docBody.Append(GetNewParagraph(phrase, before: 240, left: 992, aligment: ParagraphAligment.Justifie));
            }

            Paragraph paragraph = new Paragraph();
            PlaceTextAtCoordinate(paragraph, "CURRICULUM VITAE", -45.57, 72.57);
            docBody.Append(paragraph);
        }

        

        private void PlaceTextAtCoordinate(Paragraph para, string text, double xCoordinate, double uCoordinate)
        {
            var picRun = para.AppendChild(new Run());

            Picture picture1 = picRun.AppendChild(new Picture());

            Shapetype shapetype1 = new Shapetype() { Id = "_x0000_t202", CoordinateSize = "21600,21600", OptionalNumber = 202, EdgePath = "m,l,21600r21600,l21600,xe" };
            Stroke stroke1 = new Stroke() { JoinStyle = StrokeJoinStyleValues.Miter };
            DocumentFormat.OpenXml.Vml.Path path1 = new DocumentFormat.OpenXml.Vml.Path() { AllowGradientShape = true, ConnectionPointType = ConnectValues.Rectangle };

            shapetype1.Append(stroke1);
            shapetype1.Append(path1);

            Shape shape1 = new Shape() { Id = "Text Box 2", Style = string.Format("position:absolute;margin-left:{0:F1}pt;margin-top:{1:F1}pt;width:39.52pt;height:399.4pt;z-index:251657216;visibility:visible;mso-wrap-style:square;mso-wrap-distance-left:9pt;mso-wrap-distance-top:3.6pt;mso-wrap-distance-right:9pt;mso-wrap-distance-bottom:3.6pt;mso-position-horizontal-relative:text;mso-position-vertical-relative:text;mso-width-relative:margin;mso-height-relative:margin;v-text-anchor:top", xCoordinate, uCoordinate), Stroked = false };

            TextBox textBox1 = new TextBox() { Style = "layout-flow:vertical;mso-layout-flow-alt:bottom-to-top;mso-fit-shape-to-text:t" };

            TextBoxContent textBoxContent1 = new TextBoxContent();
            Paragraph textParagraph = GetNewParagraph(text, fontName: "Arial", fontSize: 28, fontColor: "BF96DE");
            AddCharacterSpacingToElement(textParagraph, 240);
            textBoxContent1.Append(textParagraph);

            textBox1.Append(textBoxContent1);
            DocumentFormat.OpenXml.Vml.Wordprocessing.TextWrap textWrap1 = new DocumentFormat.OpenXml.Vml.Wordprocessing.TextWrap() { Type = DocumentFormat.OpenXml.Vml.Wordprocessing.WrapValues.Square };

            shape1.Append(textBox1);
            shape1.Append(textWrap1);

            picture1.Append(shapetype1);
            picture1.Append(shape1);
        }

        private static Footer GenerateFooter()
        {
            var element =
                new Footer(
                    new SdtBlock(
                        new SdtProperties(
                            new SdtId() { Val = 317275692 }),
                        new SdtContentBlock(
                            new Paragraph(
                                new ParagraphProperties(
                                    new ParagraphStyleId() { Val = "Footer" },
                                    new Justification() { Val = JustificationValues.Center }),
                                new Run(
                                    new RunProperties(
                                        new NoProof(),
                                        new Languages() { EastAsia = "en-NZ" })),
                                new SimpleField(
                                    new Run(
                                        new RunProperties(
                                            new NoProof()),
                                        new Text("1")
                                    )
                                    { RsidRunAddition = "001F06F5" }
                                )
                                { Instruction = " PAGE   \\* MERGEFORMAT " }
                            )
                            { RsidParagraphAddition = "00F1559F", RsidParagraphProperties = "00F1559F", RsidRunAdditionDefault = "00F1559F" })),
                    new Paragraph(
                        new ParagraphProperties(
                            new ParagraphStyleId() { Val = "Footer" },
                            new Tabs(
                                new TabStop() { Val = TabStopValues.Clear, Position = 4320 },
                                new TabStop() { Val = TabStopValues.Clear, Position = 8640 },
                                new TabStop() { Val = TabStopValues.Center, Position = 4820 },
                                new TabStop() { Val = TabStopValues.Right, Position = 9639 }))
                    )
                    { RsidParagraphMarkRevision = "005D2110", RsidParagraphAddition = "002D26D8", RsidParagraphProperties = "007F0645", RsidRunAdditionDefault = "002D26D8" });
            return element;
        }

        private Border GenerateBorder(string color, bool showTop, bool showBottom, bool showLeft, bool showRight)
        {
            Border border = new Border();

            LeftBorder leftBorder = new LeftBorder() {
                Color = new StringValue(color),
                Size = 1,
                Val = new EnumValue<BorderValues>(BorderValues.Single)
            };

            RightBorder rightBorder = new RightBorder()
            {
                Color = new StringValue(color),
                Size = 1,
                Val = new EnumValue<BorderValues>(BorderValues.Single)
            };

            TopBorder topBorder = new TopBorder()
            {
                Color = new StringValue(color),
                Size = 1,
                Val = new EnumValue<BorderValues>(BorderValues.Single)
            };

            BottomBorder bottomBorder = new BottomBorder()
            {
                Color = new StringValue(color),
                Size = 1,
                Val = new EnumValue<BorderValues>(BorderValues.Single)
            };

            if (showTop) { border.Append(topBorder); }
            if (showBottom) { border.Append(bottomBorder); }
            if (showLeft) { border.Append(leftBorder); }
            if (showRight) { border.Append(rightBorder); }
            return border;
        }

        private void CreateDomainesDInterventionSection(Body docBody)
        {
            Table tableItems;
            TableRow domainesRow;
            TableCell firstColumn, secondColumn;

            Paragraph titreParagraph, itemParagraphModele, currentParagraphItem;
            Run titreRun, itemRun;

            bool twoColumns = false;

            var domaines = utilisateur.Conseiller.DomaineDInterventions.Where(x => !String.IsNullOrEmpty(x.Description)).ToList();
            string[] domaineItens = domaines.Select(x => x.Description).ToArray();
            
            titreParagraph = new Paragraph();
            AddIndentationToElement(titreParagraph, 993, 0);
            AddSpacingToElement(titreParagraph, 240, 120, spaceSimple);

            titreRun = new Run();
            titreRun.Append(GetRunProperties("Arial", mauveCode, "22", true, false));
            titreRun.Append(new Text("PRINCIPAUX DOMAINES D’INTERVENTION"));
            titreParagraph.Append(titreRun);

            tableItems = new Table();

            TableProperties tableProperties = new TableProperties(new TableBorders(new TopBorder
            {
                Color = new StringValue(mauveCode),
                Size = 1,
                Val = new EnumValue<BorderValues>(BorderValues.Dotted)
            },
            new BottomBorder
            {
                Color = new StringValue(mauveCode),
                Size = 1,
                Val = new EnumValue<BorderValues>(BorderValues.Dotted)
            }),
            new TableWidth
            {
                Width = "9382",
                Type = TableWidthUnitValues.Dxa
            },
            new TableIndentation
            {
                Width = 959,
                Type = TableWidthUnitValues.Dxa
            },
            new TableCellMarginDefault
            {
                TableCellRightMargin = new TableCellRightMargin { Width = 108, Type = new EnumValue<TableWidthValues>(TableWidthValues.Dxa) },
                TableCellLeftMargin = new TableCellLeftMargin { Width = 108, Type = new EnumValue<TableWidthValues>(TableWidthValues.Dxa) }
            });

            tableItems.AppendChild<TableProperties>(tableProperties);

            domainesRow = new TableRow();
            firstColumn = new TableCell();
            secondColumn = new TableCell();

            itemParagraphModele = new Paragraph();
            ParagraphProperties paragraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = "Puce1" },
                Justification = new Justification { Val = new EnumValue<JustificationValues>(JustificationValues.Left) }
            };

            itemParagraphModele.Append(paragraphProperties);

            twoColumns = domaineItens.Length > 4;
            int count = 0;
            int right = domaineItens.Length / 2;
            int left = domaineItens.Length - right;
            int spaceTop = 0;
            int spaceBotton = 0;
            foreach (string domaine in domaineItens)
            {
                itemRun = new Run();
                itemRun.Append(GetRunProperties("Arial", "Black", "20", false, false), new Text(domaine));
                currentParagraphItem = (Paragraph)itemParagraphModele.CloneNode(true);
                spaceTop = count == 0 || (twoColumns && count == left) ? 120 : 0;
                spaceBotton = count == domaineItens.Length - 1 || (twoColumns && count == left - 1) ? 120 : 0;
                AddSpacingToElement(currentParagraphItem, spaceTop, spaceBotton, spaceSimple);
                currentParagraphItem.Append(itemRun);

                if (!twoColumns || count < left)
                    firstColumn.Append(currentParagraphItem);
                else
                    secondColumn.Append(currentParagraphItem);
                count++;
            }
            
            if (twoColumns)
                domainesRow.Append(firstColumn, secondColumn);
            else
                domainesRow.Append(firstColumn);

            tableItems.Append(domainesRow);

            docBody.Append(titreParagraph, tableItems);
        }

        private void CreateFormationAcademiqueEtCertificationSection(Body docBody)
        {
            bool HasCertification = utilisateur.Conseiller.Formations.Any(x => x.Type != null && x.Type.Description == "Certification" && !String.IsNullOrEmpty(x.Description));

            Table tableFormationAcademiqueEtCertification;
            TableProperties tableProperties;
            TableGrid tableGrid;
            TableRow titreRow, rowModele;

            TableCell formationTitreCell, certificationTitreCell, formationAcademiqueCellModele, certificationCell;
            TableCellProperties formationCellPropertiesModele, certificationCellPropertiesModele = null;

            Paragraph formationTitreParagraph, certificationTitreParagraph, diplomeParagraphModele, instituitionParagraphModele, certificationItemParagraphModele;
            Run formationTitreRun, certificationTitreRun, diplomeModeleRun, instituitionModeleRun, certificationItemRun;
            ParagraphProperties paragraphProperties;

            tableFormationAcademiqueEtCertification = new Table();

            tableProperties = new TableProperties();
            tableProperties.Append(
                new TableIndentation {
                    Width = 959,
                    Type = new EnumValue<TableWidthUnitValues>(TableWidthUnitValues.Dxa)
                },
                new TableLook {
                    Val = "04A0",
                    NoVerticalBand = new OnOffValue(true),
                    LastColumn = new OnOffValue(false),
                    FirstColumn = new OnOffValue(true),
                    LastRow = new OnOffValue(false),
                    FirstRow = new OnOffValue(true)
                },
                new TableCellMarginDefault
                {
                    TableCellRightMargin = new TableCellRightMargin { Width = 108, Type = new EnumValue<TableWidthValues>(TableWidthValues.Dxa) },
                    TableCellLeftMargin = new TableCellLeftMargin { Width = 108, Type = new EnumValue<TableWidthValues>(TableWidthValues.Dxa) }
                });

            tableGrid = new TableGrid();
            tableGrid.Append(new GridColumn { Width = new StringValue("4679") }, new GridColumn { Width = new StringValue("4689") });

            tableFormationAcademiqueEtCertification.Append(tableProperties, tableGrid);

            titreRow = new TableRow();
            formationAcademiqueCellModele = null;

            formationTitreCell = new TableCell();
            formationTitreParagraph = new Paragraph();
            AddSpacingToElement(formationTitreParagraph, 120, 120, spaceSimple);
            AddIndentationToElement(formationTitreParagraph, -108, 0);
            formationTitreRun = new Run();

            formationCellPropertiesModele = new TableCellProperties();
            formationCellPropertiesModele.Append(new TableCellWidth { Width = new StringValue("4679"), Type = new EnumValue<TableWidthUnitValues>(TableWidthUnitValues.Dxa) });

            formationTitreRun.Append(GetRunProperties("Arial", mauveCode, "22", true, false));
            formationTitreRun.Append(new Text("FORMATION ACADÉMIQUE"));
            formationTitreParagraph.Append(formationTitreRun);
            formationTitreCell.Append(formationCellPropertiesModele.CloneNode(true), formationTitreParagraph);
            titreRow.Append(formationTitreCell);

            if (HasCertification)
            {
                certificationTitreCell = new TableCell();
                certificationTitreParagraph = new Paragraph();
                AddSpacingToElement(certificationTitreParagraph, 120, 120, spaceSimple);
                AddIndentationToElement(certificationTitreParagraph, 0, 0);
                certificationTitreRun = new Run();

                certificationCellPropertiesModele = new TableCellProperties();
                certificationCellPropertiesModele.Append(new TableCellWidth { Width = new StringValue("4689"), Type = new EnumValue<TableWidthUnitValues>(TableWidthUnitValues.Dxa) });

                certificationTitreRun.Append(GetRunProperties("Arial", mauveCode, "22", true, false));
                certificationTitreRun.Append(new Text("CERTIFICATIONS"));
                certificationTitreParagraph.Append(certificationTitreRun);
                certificationTitreCell.Append(certificationCellPropertiesModele.CloneNode(true), certificationTitreParagraph);
                titreRow.Append(certificationTitreCell);
            }

            tableFormationAcademiqueEtCertification.Append(titreRow);

            foreach (FormationScolaire formation in utilisateur.Conseiller.FormationsScolaires)
            {
                formationAcademiqueCellModele = new TableCell();
                diplomeParagraphModele = new Paragraph();
                AddSpacingToElement(diplomeParagraphModele, 0, 0, spaceSimple);
                AddIndentationToElement(diplomeParagraphModele, -108, 0);
                instituitionParagraphModele = new Paragraph();
                AddSpacingToElement(instituitionParagraphModele, 0, 0, spaceSimple);
                AddIndentationToElement(instituitionParagraphModele, -108, 0);
                diplomeModeleRun = new Run();
                instituitionModeleRun = new Run();

                diplomeModeleRun.Append(GetRunProperties("Arial", "Black", "20", true, false));
                instituitionModeleRun.Append(GetRunProperties("Arial", "Black", "20", false, true));

                diplomeModeleRun.Append(new Text($"{formation.Diplome} ({formation.DateConclusion})"));
                instituitionModeleRun.Append(new Text(formation.Ecole.Nom));

                diplomeParagraphModele.Append(diplomeModeleRun);
                instituitionParagraphModele.Append(instituitionModeleRun);

                formationAcademiqueCellModele.Append(formationCellPropertiesModele.CloneNode(true), diplomeParagraphModele, instituitionParagraphModele);

                rowModele = new TableRow();

                rowModele.Append(formationAcademiqueCellModele);
                tableFormationAcademiqueEtCertification.Append(rowModele);
            }

            if (HasCertification)
            {
                TableRow firstRow = (TableRow)tableFormationAcademiqueEtCertification.Where(x => x is TableRow).Skip(1).First();
                certificationCell = new TableCell();
                certificationCell.Append(certificationCellPropertiesModele.CloneNode(true));
                foreach (Formation formation in utilisateur.Conseiller.Formations.Where(x => x.Type != null && x.Type.Description == "Certification" && !String.IsNullOrEmpty(x.Description)))
                {
                    certificationItemParagraphModele = new Paragraph();
                    certificationItemRun = new Run();
                    certificationItemRun.Append(GetRunProperties("Arial", "Black", "20", false, false), new Text(formation.Description));

                    paragraphProperties = new ParagraphProperties
                    {
                        ParagraphStyleId = new ParagraphStyleId { Val = "Puce1" },
                        Justification = new Justification { Val = new EnumValue<JustificationValues>(JustificationValues.Left) }
                    };
                    certificationItemParagraphModele.Append(paragraphProperties, certificationItemRun);
                    certificationCell.Append(certificationItemParagraphModele);
                    AddIndentationToElement(certificationItemParagraphModele, 743, 0);
                    AddSpacingToElement(certificationItemParagraphModele, 0, 0, spaceSimple);
                }

                firstRow.Append(certificationCell);
            }

            Paragraph blankParagraph = new Paragraph();
            AddSpacingToElement(blankParagraph, 0, 0, spaceSimple);
            docBody.Append(blankParagraph);

            docBody.Append(tableFormationAcademiqueEtCertification);
        }

        private void CreateResumeDIntervention(Body docBody)
        {
            Table tableResumeIntervention;
            TableProperties tableProperties;
            TableRow headerRow, employeurRowModele, mandatRowModele;
            TableRowProperties tableRowProperties;
            Paragraph headerParagraph, mandatParagraphModele, currentParagraph;
            Run headerRun, mandatRunModele, currentRun;
            RunProperties runPropertiesModele;

            TableCell currentCell;
            TableCellProperties cellProperties;
            VerticalMerge verticalMerge;
            string previousClient = "";
            string previousFonction = "";

            //Nouvelle page et titre

            docBody.Append(new Paragraph(new Run(new Break { Type = new EnumValue<BreakValues>(BreakValues.Page) })));
            currentParagraph = new Paragraph();
            AddSpacingToElement(currentParagraph, 0, 120, spaceSimple);
            AddIndentationToElement(currentParagraph, -283, 0);
            currentRun = new Run();
            currentRun.Append(GetRunProperties("Arial Gras", mauveCode, "22", true, false));
            currentRun.Append(new Text("RÉSUMÉ DES INTERVENTIONS"));
            currentParagraph.Append(currentRun);
            docBody.Append(currentParagraph);

            //Fin nouvelle page et titre

            tableResumeIntervention = new Table();

            string borderColor = "595959";

            //Configuration du tableau

            tableProperties = new TableProperties();
            tableProperties.Append(new TableCellMarginDefault
            {
                TableCellRightMargin = new TableCellRightMargin { Width = 70, Type = new EnumValue<TableWidthValues>(TableWidthValues.Dxa) },
                TableCellLeftMargin = new TableCellLeftMargin { Width = 70, Type = new EnumValue<TableWidthValues>(TableWidthValues.Dxa) }
            });
            tableProperties.Append(new TableBorders
            {
                TopBorder = new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4, Space = 0, Color = borderColor },
                LeftBorder = new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4, Space = 0, Color = borderColor },
                BottomBorder = new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4, Space = 0, Color = borderColor },
                RightBorder = new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4, Space = 0, Color = borderColor },
                InsideHorizontalBorder = new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Dotted), Size = 4, Space = 0, Color = borderColor },
                InsideVerticalBorder = new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Dotted), Size = 4, Space = 0, Color = borderColor }
            });
            tableProperties.Append(new TableWidth { Width = "10982", Type = new EnumValue<TableWidthUnitValues>(TableWidthUnitValues.Dxa) });
            tableProperties.Append(new TableJustification { Val = new EnumValue<TableRowAlignmentValues>(TableRowAlignmentValues.Center) });
            tableProperties.Append(new TableLayout { Type = new EnumValue<TableLayoutValues>(TableLayoutValues.Fixed) });
            
            tableProperties.Append(new TableGrid(
                            new GridColumn { Width = "539" },
                            new GridColumn { Width = "2228" },
                            new GridColumn { Width = "2840" },
                            new GridColumn { Width = "1417" },
                            new GridColumn { Width = "1701" },
                            new GridColumn { Width = "1134" },
                            new GridColumn { Width = "1123" }
                         )
            );

            tableResumeIntervention.Append(tableProperties);

            //Fin de la configation du tableau

            headerRow = new TableRow();
            tableRowProperties = new TableRowProperties();
            tableRowProperties.Append(new TableJustification { Val = new EnumValue<TableRowAlignmentValues>(TableRowAlignmentValues.Center) });
            headerRow.Append(tableRowProperties);

            headerParagraph = new Paragraph();
            headerRun = new Run();

            runPropertiesModele = GetRunProperties("Arial", "White", "18", true, false);
            headerRun.Append(runPropertiesModele);

            new string[] { "N°", "CLIENT", "PROJET", "ENVERGURE (J-P)", "FONCTION", "ANNÉE", "EFFORTS (MOIS)" }.ToList().ForEach(x =>
            {
                currentParagraph = (Paragraph)headerParagraph.CloneNode(true);
                AddSpacingToElement(currentParagraph, 40, 40, spaceSimple);
                AddAligmentToParagrah(currentParagraph, ParagraphAligment.Centre);
                currentRun = (Run)headerRun.CloneNode(true);

                currentRun.Append(new Text(x));
                currentParagraph.Append(currentRun);

                currentCell = GetNewCell(currentParagraph, GetCellProperty(mauveCode, "506", TableRowAlignmentValues.Center, TableVerticalAlignmentValues.Center));
                headerRow.Append(currentCell);
            });

            tableResumeIntervention.Append(headerRow);
            var sss = utilisateur.Conseiller.Mandats;
            
            var mandatsByEmployeur = from data in utilisateur.Conseiller.Mandats.OrderByDescending(x => Int32.TryParse(x.Numero, out int numero))
                                     group data by new { data.Projet.SocieteDeConseil.Nom } into g
                                     select g;

            foreach (var employeur in mandatsByEmployeur)
            {
                employeurRowModele = new TableRow();
                tableRowProperties = new TableRowProperties();
                tableRowProperties.Append(new CantSplit(), new Justification { Val = new EnumValue<JustificationValues>(JustificationValues.Center) });

                employeurRowModele.Append(tableRowProperties);

                currentCell = new TableCell();
                cellProperties = GetCellProperty("F2F2F2", "10986", TableRowAlignmentValues.Left, TableVerticalAlignmentValues.Center);
                cellProperties.Append(new GridSpan { Val = 7 });
                cellProperties.Append(new TableCellBorders
                {
                    TopBorder = new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4, Color = "595959" },
                    BottomBorder = new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4, Color = "595959" }
                });

                currentCell.Append(cellProperties);
                currentCell.Append(GetNewParagraph(employeur.Key.Nom.ToUpper(), fontName: "Arial Gras", fontSize: 20, isBold: true, before:40, after: 40));
                employeurRowModele.Append(currentCell);

                tableResumeIntervention.Append(employeurRowModele);
                foreach (var mandat in employeur)
                {
                    mandatRowModele = new TableRow();
                    tableRowProperties = new TableRowProperties();
                    tableRowProperties.Append(new TableRowHeight { Val = 293 });
                    tableRowProperties.Append(new Justification { Val = new EnumValue<JustificationValues>(JustificationValues.Center) });

                    mandatRowModele.Append(tableRowProperties);

                    //Chaque colonne a une largeur différent, donc, une liste avec le valeur et la larguer est créée
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>(mandat.Numero, "506"),
                        new KeyValuePair<string, string>(mandat.Projet.Client.Nom, "2262"),
                        new KeyValuePair<string, string>(mandat.Projet.Nom, "2842"),
                        new KeyValuePair<string, string>(mandat.Projet.Envergure.ToString(), "1275"),
                        new KeyValuePair<string, string>(mandat.Fonction.Description, "1843"),
                        new KeyValuePair<string, string>(mandat.DateDebut.Year == mandat.DateFin.Year ? $"{mandat.DateDebut.Year}" : $"{mandat.DateDebut.Year} - {mandat.DateFin.Year}", "1134"),
                        new KeyValuePair<string, string>(mandat.Efforts.ToString(), "1124")
                    }
                    .ToList().ForEach(x =>
                    {
                        mandatParagraphModele = new Paragraph();
                        AddSpacingToElement(mandatParagraphModele, 40, 40, spaceSimple);
                        AddAligmentToParagrah(mandatParagraphModele, ParagraphAligment.Centre);

                        mandatRunModele = new Run();
                        mandatRunModele.Append(GetRunProperties("Arial", "Black", "18", false, false));
                        mandatRunModele.Append(new Text(x.Key));

                        currentCell = new TableCell();
                        cellProperties = GetCellProperty("White", x.Value, TableRowAlignmentValues.Center, TableVerticalAlignmentValues.Center);

                        if (x.Value == "2262")
                        { // mandat.Projet.Client.Nom
                            verticalMerge = new VerticalMerge()
                            {
                                Val = x.Key.Equals(previousClient) ? MergedCellValues.Continue : MergedCellValues.Restart
                            };
                            cellProperties.Append(verticalMerge);
                        }

                        if (x.Value == "1843")
                        { // mandat.Fonction.Description
                            verticalMerge = new VerticalMerge()
                            {
                                Val = x.Key.Equals(previousFonction) ? MergedCellValues.Continue : MergedCellValues.Restart
                            };
                            cellProperties.Append(verticalMerge);
                        }

                        currentCell.Append(cellProperties);
                        mandatParagraphModele.Append(mandatRunModele);
                        currentCell.Append(mandatParagraphModele);
                        mandatRowModele.Append(currentCell);
                    });

                    tableResumeIntervention.Append(mandatRowModele);
                    previousClient = mandat.Projet.Client.Nom;
                    previousFonction = mandat.Fonction.Description;
                }
                previousClient = "";
                previousFonction = "";
            }
            docBody.Append(tableResumeIntervention);
        }

        private void CreateMandats(Body docBody)
        {
            Paragraph employeurParagraphModele, currentParagraph;
            ParagraphProperties paragraphProperties;

            Run currentRun;

            Table tableInfoMandat;
            TableProperties tableProperties;
            TableRow tableRow;

            TableGrid tableGrid = new TableGrid();
                        tableGrid.Append(new GridColumn { Width = "1600" });
                        tableGrid.Append(new GridColumn { Width = "8818" });

            TableLook tableLook = new TableLook
            {
                FirstRow = new OnOffValue(false),
                LastRow = new OnOffValue(false),
                FirstColumn = new OnOffValue(false),
                LastColumn = new OnOffValue(false),
                NoHorizontalBand = new OnOffValue(false),
                NoVerticalBand = new OnOffValue(false)
            };

            docBody.Append(new Paragraph(new Run(new Break { Type = new EnumValue<BreakValues>(BreakValues.Page) })));

            var mandatsByEmployeur = from data in utilisateur.Conseiller.Mandats.OrderByDescending(x => Int32.TryParse(x.Numero, out int numero))
                                     group data by data.Projet.SocieteDeConseil.Nom into g
                                     select g;

            foreach (var employeur in mandatsByEmployeur)
            {
                employeurParagraphModele = new Paragraph();
                paragraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = "Titre1" }
                };
                AddSpacingToElement(paragraphProperties, 240, 240, spaceSimple);
                employeurParagraphModele.Append(paragraphProperties);
                employeurParagraphModele.Append(new Run(new Text(employeur.Key)));

                docBody.Append(employeurParagraphModele);

                var mandatsByClientsOfAnEmployeur = employeur.OrderByDescending(x => Int32.TryParse(x.Numero, out int numero));

                foreach (var mandat in mandatsByClientsOfAnEmployeur)
                {
                    docBody.Append(GetTitre(mandat.Projet.Client.Nom, style: "Titre2"));

                    tableInfoMandat = new Table();
                    tableProperties = new TableProperties();
                    tableProperties.Append(new TableWidth { Width = "10418", Type = new EnumValue<TableWidthUnitValues>(TableWidthUnitValues.Dxa) });
                    tableProperties.Append(new TableLayout { Type = new EnumValue<TableLayoutValues>(TableLayoutValues.Fixed) });
                    tableProperties.Append(new TableCellMarginDefault
                    {
                        TableCellRightMargin = new TableCellRightMargin { Width = 70, Type = new EnumValue<TableWidthValues>(TableWidthValues.Dxa) },
                        TableCellLeftMargin = new TableCellLeftMargin { Width = 70, Type = new EnumValue<TableWidthValues>(TableWidthValues.Dxa) }
                    },
                    new TableIndentation
                    {
                        Width = 0,
                        Type = TableWidthUnitValues.Dxa
                    });

                    tableInfoMandat.Append(tableProperties);

                    tableInfoMandat.Append(tableGrid.CloneNode(true));

                    tableInfoMandat.Append(tableLook.CloneNode(true));

                    paragraphProperties = new ParagraphProperties();
                    paragraphProperties.Append(new SpacingBetweenLines
                    {
                        Before = "0",
                        After = "0",
                        BeforeAutoSpacing = new OnOffValue(false),
                        AfterAutoSpacing = new OnOffValue(false),
                        Line = "233"
                    });

                    tableRow = new TableRow();
                    tableRow.Append(new TableCell(new Paragraph(paragraphProperties.CloneNode(true), new Run(GetRunProperties("Arial", "Black", "20", false, false), new Text("Mandat no:")))));
                    tableRow.Append(new TableCell(new Paragraph(paragraphProperties.CloneNode(true), new Run(GetRunProperties("Arial", "Black", "20", false, false), new Text(mandat.Numero)))));
                    tableInfoMandat.Append(tableRow);

                    tableRow = new TableRow();
                    tableRow.Append(new TableCell(new Paragraph(paragraphProperties.CloneNode(true), new Run(GetRunProperties("Arial", "Black", "20", true, false), new Text("Projet:")))));
                    tableRow.Append(new TableCell(new Paragraph(paragraphProperties.CloneNode(true), new Run(GetRunProperties("Arial", "Black", "20", true, false), new Text(mandat.Projet.Nom)))));
                    tableInfoMandat.Append(tableRow);

                    tableRow = new TableRow();
                    tableRow.Append(new TableCell(new Paragraph(paragraphProperties.CloneNode(true), new Run(GetRunProperties("Arial", "Black", "20", false, false), new Text("Envergure:")))));
                    tableRow.Append(new TableCell(new Paragraph(paragraphProperties.CloneNode(true), new Run(GetRunProperties("Arial", "Black", "20", false, false), new Text(mandat.Projet.Envergure.ToString())))));
                    tableInfoMandat.Append(tableRow);

                    tableRow = new TableRow();
                    tableRow.Append(new TableCell(new Paragraph(paragraphProperties.CloneNode(true), new Run(GetRunProperties("Arial", "Black", "20", false, false), new Text("Fonction:")))));
                    tableRow.Append(new TableCell(new Paragraph(paragraphProperties.CloneNode(true), new Run(GetRunProperties("Arial", "Black", "20", false, false), new Text(mandat.Fonction.Description)))));
                    tableInfoMandat.Append(tableRow);

                    tableRow = new TableRow();
                    tableRow.Append(new TableCell(new Paragraph(paragraphProperties.CloneNode(true), new Run(GetRunProperties("Arial", "Black", "20", false, false), new Text("Période:")))));
                    tableRow.Append(new TableCell(new Paragraph(paragraphProperties.CloneNode(true), new Run(GetRunProperties("Arial", "Black", "20", false, false), new Text($"{mandat.DateDebut.ToString("MMMM")} de {mandat.DateDebut.Year} à {mandat.DateFin.ToString("MMMM")} de {mandat.DateFin.Year}")))));
                    tableInfoMandat.Append(tableRow);

                    tableRow = new TableRow();
                    tableRow.Append(new TableCell(new Paragraph(paragraphProperties.CloneNode(true), new Run(GetRunProperties("Arial", "Black", "20", false, false), new Text("Efforts:")))));
                    tableRow.Append(new TableCell(new Paragraph(paragraphProperties.CloneNode(true), new Run(GetRunProperties("Arial", "Black", "20", false, false), new Text(mandat.Efforts.ToString())))));
                    tableInfoMandat.Append(tableRow);

                    tableRow = new TableRow();
                    tableRow.Append(new TableCell(new Paragraph(paragraphProperties.CloneNode(true), new Run(GetRunProperties("Arial", "Black", "20", false, false), new Text("Référence:")))));
                    tableRow.Append(new TableCell(new Paragraph(paragraphProperties.CloneNode(true), new Run(GetRunProperties("Arial", "Black", "20", false, false), new Text(mandat.Projet.NomReference)))));
                    tableInfoMandat.Append(tableRow);

                    docBody.Append(tableInfoMandat);

                    currentParagraph = new Paragraph();
                    paragraphProperties = new ParagraphProperties();
                    paragraphProperties.Append(new SpacingBetweenLines
                    {
                        After = "0",
                        Before = "120",
                        Line = spaceSimple.ToString()
                    });

                    paragraphProperties.Append(new Justification { Val = new EnumValue<JustificationValues>(JustificationValues.Both) });

                    currentParagraph.Append(paragraphProperties.CloneNode(true));

                    currentRun = new Run(GetRunProperties("Arial", "Black", "20", false, false));
                    currentRun.Append(new Text(mandat.Projet.Description));
                    currentParagraph.Append(currentRun);

                    docBody.Append(currentParagraph);


                    //Tâches

                    currentParagraph = new Paragraph();
                    paragraphProperties = new ParagraphProperties();
                    paragraphProperties.Append(new SpacingBetweenLines
                    {
                        After = "60",
                        Before = "120",
                        Line = spaceSimple.ToString()
                    });

                    paragraphProperties.Append(new Justification { Val = new EnumValue<JustificationValues>(JustificationValues.Both) });
                    currentParagraph.Append(paragraphProperties.CloneNode(true));

                    currentRun = new Run(GetRunProperties("Arial", "Black", "20", false, false));
                    currentRun.Append(new Text($"M. {utilisateur.Nom} a effectué les tâches suivantes :"));
                    currentParagraph.Append(currentRun);

                    docBody.Append(currentParagraph);

                    foreach (var tache in mandat.Taches)
                    {
                        currentParagraph = new Paragraph();
                        currentParagraph.Append(new ParagraphProperties(new Justification { Val = new EnumValue<JustificationValues>(JustificationValues.Both) }, new SpacingBetweenLines
                        {
                            After = "0",
                            Before = "0",
                            Line = spaceSimple.ToString()
                        })
                        { ParagraphStyleId = new ParagraphStyleId { Val = "Puce1" } });

                        currentRun = new Run(GetRunProperties("Arial", "Black", "20", false, false));
                        currentRun.Append(new Text(tache.Description ?? ""));
                        currentParagraph.Append(currentRun);
                        docBody.Append(currentParagraph);
                    }

                    //End tâches

                    if (mandat.Projet.Technologies != null)
                    {
                        currentParagraph = new Paragraph();
                        paragraphProperties = new ParagraphProperties();
                        paragraphProperties.Append(new SpacingBetweenLines
                        {
                            After = "0",
                            Before = "120",
                            Line = spaceSimple.ToString()
                        });

                        paragraphProperties.Append(new Justification { Val = new EnumValue<JustificationValues>(JustificationValues.Both) });
                        currentParagraph.Append(paragraphProperties.CloneNode(true));

                        currentRun = new Run(GetRunProperties("Arial", "Black", "20", false, false));
                        currentRun.Append(new Text("Environnement technologique: " + String.Join(", ", mandat.Projet.Technologies.Select(x => x.Nom.ToUpper()))));
                        currentParagraph.Append(currentRun);

                        docBody.Append(currentParagraph);
                    }
                }
            }
        }

        private void CreateTechnologies(Body docBody)
        {
            var technologisByCategorie = from data in utilisateur.Conseiller.Technologies
                                         group data by data.Categorie.Nom into g
                                     select g;

            Table tableTechnologies = new Table();
            TableProperties tableProperties = new TableProperties();
            TableRow tableRow;

            docBody.Append(new Paragraph(new Run(new Break { Type = new EnumValue<BreakValues>(BreakValues.Page) })));
            tableProperties.Append(new TableWidth { Width = "10131", Type = new EnumValue<TableWidthUnitValues>(TableWidthUnitValues.Dxa) });
            tableProperties.Append(new TableLayout { Type = new EnumValue<TableLayoutValues>(TableLayoutValues.Fixed) });
            tableProperties.Append(new TableCellMargin
                (
                    new LeftMargin { Width = "70", Type = new EnumValue<TableWidthUnitValues>(TableWidthUnitValues.Dxa) },
                    new RightMargin { Width = "70", Type = new EnumValue<TableWidthUnitValues>(TableWidthUnitValues.Dxa) }
                ));
            tableTechnologies.Append(tableProperties);
            TableGrid tableGrid = new TableGrid();
            tableGrid.Append(new GridColumn { Width = "3730" });
            tableGrid.Append(new GridColumn { Width = "1049" });
            tableGrid.Append(new GridColumn { Width = "578" });
            tableGrid.Append(new GridColumn { Width = "3725" });
            tableGrid.Append(new GridColumn { Width = "1049" });
            tableTechnologies.Append(tableGrid);

            TableLook tableLook = new TableLook
            {
                FirstRow = new OnOffValue(false),
                LastRow = new OnOffValue(false),
                FirstColumn = new OnOffValue(false),
                LastColumn = new OnOffValue(false),
                NoHorizontalBand = new OnOffValue(false),
                NoVerticalBand = new OnOffValue(false)
            };
            tableTechnologies.Append(tableLook);

            // Titres
            tableRow = new TableRow();

            tableRow.Append(GetCell(GetNewParagraph("TECHNOLOGIES", fontColor: mauveCode, isBold: true, before: 120, after: 120, fontSize:18, betweenLines: 276), borderColor: mauveCode, isTop: true, isBottom: true));
            tableRow.Append(GetCell(GetNewParagraph("MOIS", fontColor: mauveCode, isBold: true, before: 120, after: 120, fontSize: 18, betweenLines: 276), borderColor: mauveCode, isTop: true, isBottom: true));
            tableRow.Append(GetCell(GetNewParagraph("", fontSize: 18, isBold: true, before: 120, after: 120, betweenLines: 276)));
            tableRow.Append(GetCell(GetNewParagraph("TECHNOLOGIES", fontColor: mauveCode, isBold: true, before: 120, after: 120, fontSize: 18, betweenLines: 276), borderColor: mauveCode, isTop: true, isBottom: true));
            tableRow.Append(GetCell(GetNewParagraph("MOIS", fontColor: mauveCode, isBold: true, before: 120, after: 120, fontSize: 18, betweenLines: 276), borderColor: mauveCode, isTop: true, isBottom: true));
            tableTechnologies.Append(tableRow);

            // Technologies
            var total = (technologisByCategorie.Count()*2) + utilisateur.Conseiller.Technologies.Count();
            var droite = total / 2;
            var gauche = total - droite;
            var count = 0;
            var isFirts = true;

            foreach (var categorie in technologisByCategorie)
            {
                AddTechnologieCell(tableTechnologies, categorie.Key, null, count, isFirts);
                count++;
                isFirts = isFirts && count < gauche;
                count = count == gauche ? 0 : count;

                foreach (Technologie technologie in categorie)
                {
                    AddTechnologieCell(tableTechnologies, categorie.Key, technologie, count, isFirts);
                    count++;
                    isFirts = isFirts && count < gauche;
                    count = count == gauche ? 0 : count;
                }

                count = count > 0 ? count+1 : 0;
            }

            docBody.Append(tableTechnologies);
            docBody.Append(new Paragraph(new Run(new Break { Type = new EnumValue<BreakValues>(BreakValues.Page) })));
        }

        private void AddTechnologieCell(Table tableTechnologies, string categorie, Technologie technologie, int possition, bool isFirts)
        {
            TableRow tableRow;
            if (technologie == null)
            {
                if (isFirts)
                {
                    if (possition > 0)
                    {
                        tableRow = new TableRow();
                        tableRow.Append(GetCell(GetNewParagraph("")));
                        tableRow.Append(GetCell(GetNewParagraph("")));
                        tableRow.Append(GetCell(GetNewParagraph("")));
                        tableTechnologies.Append(tableRow);
                    }
                    tableRow = new TableRow();
                    tableRow.Append(GetCell(GetNewParagraph(categorie, fontSize: 18, isBold: true, isItalic: true), isTop: possition != 0, borderColor: mauveCode, borderValue: BorderValues.Dotted));
                    tableRow.Append(GetCell(GetNewParagraph(""), isTop: possition != 0, borderColor: mauveCode, borderValue: BorderValues.Dotted));
                    tableRow.Append(GetCell(GetNewParagraph("")));
                    tableTechnologies.Append(tableRow);
                }
                else
                {
                    tableRow = tableTechnologies.Elements<TableRow>().ElementAt(possition+1);
                    tableRow.Append(GetCell(GetNewParagraph(categorie, fontSize: 18, isBold: true, isItalic: true), isTop: possition != 0, borderColor: mauveCode, borderValue: BorderValues.Dotted));
                    tableRow.Append(GetCell(GetNewParagraph(""), isTop: possition != 0, borderColor: mauveCode, borderValue: BorderValues.Dotted));
                }                
            }
            else
            {
                if (isFirts)
                {
                    tableRow = new TableRow();
                    tableRow.Append(GetCell(GetNewParagraph(technologie.Nom, fontSize: 18)));
                    tableRow.Append(GetCell(GetNewParagraph(technologie.MoisDExperience.ToString(), fontSize: 18, aligment: ParagraphAligment.Centre)));
                    tableRow.Append(GetCell(GetNewParagraph("")));
                    tableTechnologies.Append(tableRow);
                }
                else
                {
                    tableRow = tableTechnologies.Elements<TableRow>().ElementAt(possition + 1);
                    tableRow.Append(GetCell(GetNewParagraph(technologie.Nom, fontSize: 18)));
                    tableRow.Append(GetCell(GetNewParagraph(technologie.MoisDExperience.ToString(), fontSize: 18, aligment: ParagraphAligment.Centre)));
                }
            }
        }

        private void CreatePerfectionnement(Body docBody)
        {
            var perfectionnements = utilisateur.Conseiller.Perfectionnement().Where(x => !String.IsNullOrEmpty(x.Description)).ToList();

            var HasPerfectionnement = perfectionnements.Count > 0;

            if (HasPerfectionnement) {
                Table tablePerfectionnement = new Table();
                TableProperties tableProperties = new TableProperties();
                TableRow tableRow;
                ParagraphProperties paragraphProperties;

                paragraphProperties = new ParagraphProperties();
                paragraphProperties.Append(new SpacingBetweenLines
                {
                    Before = "0",
                    After = "0",
                    BeforeAutoSpacing = new OnOffValue(false),
                    AfterAutoSpacing = new OnOffValue(false),
                    Line = "233"
                });

                tableProperties.Append(new TableWidth { Width = "10131", Type = new EnumValue<TableWidthUnitValues>(TableWidthUnitValues.Dxa) });
                tableProperties.Append(new TableLayout { Type = new EnumValue<TableLayoutValues>(TableLayoutValues.Fixed) });
                tableProperties.Append(new TableCellMargin
                    (
                        new LeftMargin { Width = "70", Type = new EnumValue<TableWidthUnitValues>(TableWidthUnitValues.Dxa) },
                        new RightMargin { Width = "70", Type = new EnumValue<TableWidthUnitValues>(TableWidthUnitValues.Dxa) }
                    ));
                tablePerfectionnement.Append(tableProperties);
                TableGrid tableGrid = new TableGrid();
                tableGrid.Append(new GridColumn { Width = "782" });
                tableGrid.Append(new GridColumn { Width = "9349" });
                tablePerfectionnement.Append(tableGrid);

                TableLook tableLook = new TableLook
                {
                    FirstRow = new OnOffValue(false),
                    LastRow = new OnOffValue(false),
                    FirstColumn = new OnOffValue(false),
                    LastColumn = new OnOffValue(false),
                    NoHorizontalBand = new OnOffValue(false),
                    NoVerticalBand = new OnOffValue(false)
                };
                tablePerfectionnement.Append(tableLook);

                docBody.Append(new Paragraph());
                docBody.Append(GetTitre("PERFECTIONNEMENT", style: "Titre2", espaceApres:240, espaceAvant:0));
                foreach (Formation perfectionnement in perfectionnements)
                {
                    tableRow = new TableRow();
                    tableRow.Append(new TableCell(new Paragraph(paragraphProperties.CloneNode(true), new Run(GetRunProperties("Arial", "Black", "20", false, false), new Text(perfectionnement.AnAcquisition.ToString())))));
                    tableRow.Append(new TableCell(new Paragraph(paragraphProperties.CloneNode(true), new Run(GetRunProperties("Arial", "Black", "20", false, false), new Text(perfectionnement.Description)))));
                    tablePerfectionnement.Append(tableRow);
                }

                docBody.Append(tablePerfectionnement);
            }
        }

        private void CreateAutresFormations(Body docBody)
        {
            var HasAutresFormations = false;

            if (HasAutresFormations)
            {
                docBody.Append(new Paragraph());
                docBody.Append(GetTitre("AUTRES FORMATIONS", style: "Titre2", espaceApres: 360, espaceAvant: 240));
            }
        }

        private void CreateAssociations(Body docBody)
        {
            var HasAssociations = utilisateur.Conseiller.Associations.Count > 0;

            if (HasAssociations)
            {
                docBody.Append(new Paragraph());
                docBody.Append(GetTitre("ASSOCIATIONS", style: "Titre2", espaceApres: 360, espaceAvant: 240));

                foreach (OrdreProfessional association in utilisateur.Conseiller.Associations)
                {
                    docBody.Append(GetPuce(association.Nom));
                }
            }
        }

        private void CreatePublications(Body docBody)
        {
            var HasPublications = utilisateur.Conseiller.Publication().Count > 0;

            if (HasPublications)
            {
                docBody.Append(new Paragraph());
                docBody.Append(GetTitre("PUBLICATIONS", style: "Titre2", espaceApres: 360, espaceAvant: 240));

                foreach (Formation publication in utilisateur.Conseiller.Publication())
                {
                    docBody.Append(GetPuce(publication.Description));
                }
            }
        }

        private void CreateConferences(Body docBody)
        {
            var HasConferences = utilisateur.Conseiller.Conference().Count > 0;

            if (HasConferences)
            {
                docBody.Append(new Paragraph());
                docBody.Append(GetTitre("CONFÉRENCES", style: "Titre2", espaceApres: 360, espaceAvant: 240));

                foreach (Formation conference in utilisateur.Conseiller.Conference()) {
                    docBody.Append(GetPuce(conference.Description));
                }
            }
        }

        private void CreateLangues(Body docBody)
        {
            var HasLangues = utilisateur.Conseiller.Langues.Count > 0;

            if (HasLangues)
            {
                docBody.Append(new Paragraph());
                docBody.Append(GetTitre("LANGUES PARLÉES, ÉCRITES", style: "Titre2", espaceApres: 240, espaceAvant: 360));
                foreach (Langue langue in utilisateur.Conseiller.Langues)
                {
                    docBody.Append(GetPuce(langue.Nom));
                    docBody.Append(GetNewParagraph("Parlé : " + langue.Parle, left: 709, before: 40));
                    docBody.Append(GetNewParagraph("Écrit : " + langue.Ecrit, left: 709, before: 40));
                    docBody.Append(GetNewParagraph("Lu : " + langue.Lu, left: 709, before: 40));
                }
            }
        }

        private Settings GenerateDocumentSettingsPart()
        {
            var element = new Settings(new EvenAndOddHeaders());
            return element;
        }

        private void ApplyFooter()
        {
            MainDocumentPart mainDocPart = document.MainDocumentPart;

            var documentSettingsPart = mainDocPart.AddNewPart<DocumentSettingsPart>("rId1");
            GenerateDocumentSettingsPart().Save(documentSettingsPart);

            FooterPart footerPart1 = mainDocPart.AddNewPart<FooterPart>("footer1");
            FooterPart footerPartEven = mainDocPart.AddNewPart<FooterPart>("footerEven");
            FooterPart footerPartOdd = mainDocPart.AddNewPart<FooterPart>("footerOdd");

            Footer footer1 = new Footer();
            footer1.Append(new Paragraph(new Run(GetFooterImage(@"images\footer.png", footerPart1))));
            footerPart1.Footer = footer1;

            footerPartEven.Footer = CreateFooterEven(footerPartEven);
            footerPartOdd.Footer = CreateFooterOdd(footerPartOdd);

            SectionProperties sectionProperties = mainDocPart.Document.Body.Descendants<SectionProperties>().FirstOrDefault();
            if (sectionProperties == null)
            {
                sectionProperties = new SectionProperties() { };
                mainDocPart.Document.Body.Append(sectionProperties);
            }
            sectionProperties.AppendChild(new TitlePage { Val = true });

            FooterReference footerReference1 = new FooterReference() { Type = HeaderFooterValues.First, Id = "footer1" };
            FooterReference footerReferenceEven = new FooterReference() { Type = HeaderFooterValues.Even, Id = "footerEven" };
            FooterReference footerReferenceOdd = new FooterReference() { Type = HeaderFooterValues.Default, Id = "footerOdd" };

            sectionProperties.Append(footerReference1, footerReferenceEven, footerReferenceOdd);
        }

        private Footer CreateFooterOdd(FooterPart footerPart)
        {
            var element =
                new Footer(
                    new Paragraph(
                        new ParagraphProperties(
                            new ParagraphStyleId() { Val = "Footer" },
                            new Tabs(
                                new TabStop() { Val = TabStopValues.Center, Position = 5103 },
                                new TabStop() { Val = TabStopValues.Right, Position = 10206 })
                                ),
                        new Run(
                            GetFooterImage(@"images\footer.png", footerPart),
                            new Text($"{utilisateur.Prenom} {utilisateur.Nom}"),
                            new TabChar(),
                            new TabChar(),
                            new SimpleField() { Instruction = " PAGE \\* MERGEFORMAT " },
                            new Text {
                                Text = " / ",
                                Space =SpaceProcessingModeValues.Preserve
                            },
                            new SimpleField() { Instruction = " NUMPAGES \\* Arabic \\* MERGEFORMAT" })
                ));

            return element;
        }

        private Footer CreateFooterEven(FooterPart footerPart)
        {
            var element =
                new Footer(
                    new Paragraph(
                        new ParagraphProperties(
                            new ParagraphStyleId() { Val = "Footer" },
                            new Tabs(
                                new TabStop() { Val = TabStopValues.Center, Position = 5103 },
                                new TabStop() { Val = TabStopValues.Right, Position = 10206 })
                                ),
                        new Run(
                            GetFooterImage(@"images\footer.png", footerPart),
                            new SimpleField() { Instruction = " PAGE \\* MERGEFORMAT " },
                            new Text
                            {
                                Text = " / ",
                                Space = SpaceProcessingModeValues.Preserve
                            },
                            new SimpleField() { Instruction = " NUMPAGES \\* Arabic \\* MERGEFORMAT" },
                            new TabChar(),
                            new TabChar(),
                            new Text($"{utilisateur.Prenom} {utilisateur.Nom}")
                            )
                ));

            return element;
        }

        private Drawing GetFooterImage(string imagePath, FooterPart footerPart)
        {
            ImagePart imagePart = footerPart.AddImagePart(ImagePartType.Png);
            using (FileStream stream = new FileStream(imagePath, FileMode.Open))
            {
                imagePart.FeedData(stream);
            }
            return DrawingManager(footerPart.GetIdOfPart(imagePart), "logoFooter", 525357, 510963, "center");
        }

    }

    public enum ParagraphAligment
    {
        Gauche,
        Droite,
        Justifie,
        Centre
    }
}
