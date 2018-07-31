using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using System.IO;

namespace WebCV_Fiches.Helpers
{
    public class WordWriter
    {
        private Utilisateur utilisateur;
        private WordprocessingDocument document;
        private const string mauveCode = "7030A0";
        private const int spaceSimple = 240;

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

        private void AddSpacing(OpenXmlCompositeElement element, int before, int after, int betweenLines)
        {
            SpacingBetweenLines spacing = new SpacingBetweenLines();

            spacing.Before = new StringValue(before.ToString());
            spacing.After = new StringValue(after.ToString());

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

        private void AddIndentation(OpenXmlCompositeElement element, int left, int right)
        {
            Indentation indentation = new Indentation();

            //if (right > 0)
            indentation.Right = new StringValue(right.ToString());

            //if (left > 0)
            indentation.Left = new StringValue(left.ToString());

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

            SpacingBetweenLines spacing = new SpacingBetweenLines();
            spacing.Before = new StringValue(before.ToString());
            spacing.After = new StringValue(after.ToString());
            spacing.Line = new StringValue(betweenLines.ToString());

            styleParagraphProperties.Append(spacing);
            styleParagraphProperties.Append(new ContextualSpacing { Val = false });
        }

        private Paragraph GetNewParagraph(string text, string styleId = "")
        {
            Paragraph paragraph = new Paragraph();

            if (!string.IsNullOrEmpty(styleId))
            {
                ParagraphProperties paragraphProperties = new ParagraphProperties();
                paragraphProperties.ParagraphStyleId = new ParagraphStyleId { Val = styleId };

                paragraph.Append(paragraphProperties);
            }

            paragraph.Append(new Run((new Text { Text = text })));
            return paragraph;
        }

        private Paragraph GetNewParagraph(string[] text, string styleId = "")
        {
            Paragraph paragraph = new Paragraph();

            if (!string.IsNullOrEmpty(styleId))
            {
                ParagraphProperties paragraphProperties = new ParagraphProperties();
                paragraphProperties.ParagraphStyleId = new ParagraphStyleId { Val = styleId };

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
                ParagraphProperties paragraphProperties = new ParagraphProperties();
                paragraphProperties.ParagraphStyleId = new ParagraphStyleId { Val = styleId };

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

        private Drawing GetNewLogoDrawing(string relationshipId)
        {
            Drawing element = new Drawing(
                        new DW.Inline
                        (
                             new DW.Extent() { Cx = 766445L, Cy = 766445L },
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
                            DistanceFromLeft = (UInt32Value)114300U,
                            DistanceFromRight = (UInt32Value)114300U,
                            EditId = "50D07946"
                            //AllowOverlap = new BooleanValue(true),
                            //LayoutInCell = new BooleanValue(true),
                            //Locked = new BooleanValue(false),
                            //BehindDoc = new BooleanValue(false),
                            //SimplePos = new BooleanValue(false)

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

            Drawing drawing = GetNewLogoDrawing(mainPart.GetIdOfPart(imagePart));
            return drawing;
        }

        private void ConfigurePage(Body docBody)
        {
            SectionProperties sectionProps = new SectionProperties();

            PageMargin pageMargins = new PageMargin();
            pageMargins.Left = 1134;
            pageMargins.Right = 1021;
            pageMargins.Bottom = 1276;
            pageMargins.Top = 1077;
            pageMargins.Footer = 454;
            pageMargins.Header = 720;
            pageMargins.Gutter = 0;

            PageSize pageSize = new PageSize();
            pageSize.Width = 12242;
            pageSize.Height = 15842;
            pageSize.Code = 1;

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
            NumberingProperties numberingProperties = new NumberingProperties();
            numberingProperties.NumberingLevelReference = new NumberingLevelReference { Val = new Int32Value(level) };
            numberingProperties.NumberingId = new NumberingId { Val = new Int32Value(bulletId) };

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
            paragraphProperties.Append(new SpacingBetweenLines { Before = "240", After = "120" });
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

        private void AddRowTotable(ref TableRow table, TableRow tableRow)
        {
            table.Append(tableRow);
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

            AddBulletToStyles(38, 0, "Puce1");

            CreateBioSection(doc.Body);
            CreateDomainesDInterventionSection(doc.Body);
            CreateFormationAcademiqueEtCertificationSection(doc.Body);
            CreateResumeDIntervention(doc.Body);
            CreateMandats(doc.Body);

            document.Save();
        }

        private void CreateBioSection(Body docBody)
        {
            Table table = new Table();
            TableRow tableRow = new TableRow();
            TableCell imageCell = new TableCell();
            TableCell nomEtFonctionCell = new TableCell();
            TableCellProperties tableCellProperties;

            Paragraph nomEtFonctionParagraph, blankParagraph, bioParagraph;

            Style bioParagraphStyle = GetNewParagraphStyle("BioP1", "BioParagraph");
            AddSpacingToStyle(bioParagraphStyle, 240, 240, 0);

            document.MainDocumentPart.StyleDefinitionsPart.Styles.Append(bioParagraphStyle);

            TableProperties tableProperties = new TableProperties(
                new TableWidth
                {
                    Width = "5000",
                    Type = TableWidthUnitValues.Pct
                },
                new TableIndentation
                {
                    Width = -459,
                    Type = TableWidthUnitValues.Dxa
                });

            table.AppendChild<TableProperties>(tableProperties);

            tableCellProperties = new TableCellProperties();
            tableCellProperties.Append(new TableCellWidth { Width = new StringValue("1360"), Type = new EnumValue<TableWidthUnitValues>(TableWidthUnitValues.Dxa) });
            imageCell.Append(tableCellProperties);
            imageCell.Append(new Paragraph(new Run(GetLogoImage(@"C:\Docs to zip\Images\logo.png"))));

            Run nomRun = new Run(), breakLine = new Run(), fonctionRun = new Run();
            nomRun.Append(GetRunProperties("Arial", mauveCode, "28", true, false));
            nomRun.Append(new Text { Text = $"{utilisateur.Prenom} {utilisateur.Nom}".Trim().ToUpper() });

            breakLine.Append(GetNewLineBreaker());

            fonctionRun.Append(GetRunProperties("Arial", "808080", "24", true, false));
            fonctionRun.Append(new Text { Text = utilisateur.Conseiller.Fonction.Description });

            nomEtFonctionParagraph = new Paragraph();
            AddSpacing(nomEtFonctionParagraph, 240, 240, 0);
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

            blankParagraph = new Paragraph();
            docBody.Append(blankParagraph);

            bioParagraph = new Paragraph();
            AddIndentation(bioParagraph, 993, 0);
            AddAligmentToParagrah(bioParagraph, ParagraphAligment.Justifie);

            Run bioRun = new Run();
            bioRun.Append(GetRunProperties("Arial", "Black", "21", false, false));
            bioRun.Append(new Text(utilisateur.Conseiller.CVs.First().ResumeExperience));

            bioParagraph.Append(bioRun);
            docBody.Append(bioParagraph);
            docBody.Append(new Paragraph());
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

        public void CreateDomainesDInterventionSection(Body docBody)
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
            AddIndentation(titreParagraph, 993, 0);

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
            });

            tableItems.AppendChild<TableProperties>(tableProperties);

            domainesRow = new TableRow();
            firstColumn = new TableCell();
            secondColumn = new TableCell();

            itemParagraphModele = new Paragraph();
            ParagraphProperties paragraphProperties = new ParagraphProperties();
            paragraphProperties.ParagraphStyleId = new ParagraphStyleId { Val = "Puce1" };
            paragraphProperties.SpacingBetweenLines = new SpacingBetweenLines { Before = "0", After = "0" };
            paragraphProperties.Justification = new Justification { Val = new EnumValue<JustificationValues>(JustificationValues.Left) };

            itemParagraphModele.Append(paragraphProperties);

            twoColumns = domaineItens.Length > 4;
            int count = 0;
            foreach (string domaine in domaineItens)
            {
                itemRun = new Run();
                itemRun.Append(GetRunProperties("Arial", "Black", "20", false, false), new Text(domaine));

                currentParagraphItem = (Paragraph)itemParagraphModele.CloneNode(true);
                currentParagraphItem.Append(itemRun);
                if (!twoColumns || count <= (domaineItens.Length / 2))
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

        public void CreateFormationAcademiqueEtCertificationSection(Body docBody)
        {
            bool HasCertification = utilisateur.Conseiller.Formations.Any(x => x.Type != null && x.Type.Description == "Certification");
            
            Table tableFormationAcademiqueEtCertification;
            TableProperties tableProperties;
            TableGrid tableGrid;
            TableRow titreRow, rowModele;

            TableCell formationTitreCell, certificationTitreCell, formationAcademiqueCellModele, certificationCell;
            TableCellProperties formationCellPropertiesModele, certificationCellPropertiesModele = null;

            Paragraph formationTitreParagraph, certificationTitreParagraph, diplomeParagraphModele, instituitionParagraphModele, certificationItemParagraphModele;
            Run formationTitreRun, certificationTitreRun, diplomeModeleRun, instituitionModeleRun, certificationItemRun;

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
                });

            tableGrid = new TableGrid();
            tableGrid.Append(new GridColumn { Width = new StringValue("4679") }, new GridColumn { Width = new StringValue("4689") });

            tableFormationAcademiqueEtCertification.Append(tableProperties, tableGrid);

            titreRow = new TableRow();
            formationAcademiqueCellModele = null;

            formationTitreCell = new TableCell();
            formationTitreParagraph = new Paragraph();
            AddSpacing(formationTitreParagraph, 120, 120, spaceSimple);
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
                AddSpacing(certificationTitreParagraph, 120, 120, spaceSimple);
                AddIndentation(certificationTitreParagraph, 283, 0);
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
                AddSpacing(diplomeParagraphModele, 0, 0, spaceSimple);
                instituitionParagraphModele = new Paragraph();
                AddSpacing(instituitionParagraphModele, 0, 0, spaceSimple);
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

                foreach (Formation formation in utilisateur.Conseiller.Formations.Where(x => x.Type.Description == "Certification"))
                {
                    certificationItemParagraphModele = new Paragraph();
                    certificationItemRun = new Run();
                    certificationItemRun.Append(GetRunProperties("Arial", "Black", "20", false, false), new Text(formation.Description));

                    ParagraphProperties paragraphProperties = new ParagraphProperties();
                    paragraphProperties.ParagraphStyleId = new ParagraphStyleId { Val = "Puce1" };
                    paragraphProperties.Justification = new Justification { Val = new EnumValue<JustificationValues>(JustificationValues.Left) };

                    certificationItemParagraphModele.Append(paragraphProperties, certificationItemRun);
                    certificationCell.Append(certificationItemParagraphModele);
                    AddIndentation(certificationItemParagraphModele, 743, 0);
                    AddSpacing(certificationItemParagraphModele, 0, 0, spaceSimple);
                }

                firstRow.Append(certificationCell);
            }

            Paragraph blankParagraph = new Paragraph();
            AddSpacing(blankParagraph, 0, 0, spaceSimple);
            docBody.Append(blankParagraph);

            docBody.Append(tableFormationAcademiqueEtCertification);
        }

        public void CreateResumeDIntervention(Body docBody)
        {
            Table tableResumeIntervention;
            TableProperties tableProperties;
            TableRow headerRow, employeurRowModele, mandatRowModele;
            TableRowProperties tableRowProperties;
            Paragraph headerParagraph, employeurParagraphModele, mandatParagraphModele, currentParagraph;
            Run headerRun, employeurRunModele, mandatRunModele, currentRun;
            RunProperties runPropertiesModele;

            TableCell currentCell;
            TableCellProperties cellProperties;

            //Nouvelle page et titre

            docBody.Append(new Paragraph(new Run(new Break { Type = new EnumValue<BreakValues>(BreakValues.Page) })));
            currentParagraph = new Paragraph();
            AddSpacing(currentParagraph, 0, 120, spaceSimple);
            AddIndentation(currentParagraph, -198, 0);
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
            tableProperties.Append(new TableBorders
            {
                TopBorder = new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4, Space = 0, Color = borderColor },
                LeftBorder = new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4, Space = 0, Color = borderColor },
                BottomBorder = new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4, Space = 0, Color = borderColor },
                RightBorder = new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4, Space = 0, Color = borderColor },
                InsideHorizontalBorder = new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Dotted), Size = 4, Space = 0, Color = borderColor },
                InsideVerticalBorder = new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Dotted), Size = 4, Space = 0, Color = borderColor }
            },
            new TableWidth { Width = "10477", Type = new EnumValue<TableWidthUnitValues>(TableWidthUnitValues.Dxa) },
            new TableJustification { Val = new EnumValue<TableRowAlignmentValues>(TableRowAlignmentValues.Center) },
            new TableLayout { Type = new EnumValue<TableLayoutValues>(TableLayoutValues.Fixed) },
            new TableCellMargin
            {
                LeftMargin = new LeftMargin { Width = "70", Type = new EnumValue<TableWidthUnitValues>(TableWidthUnitValues.Dxa) },
                RightMargin = new RightMargin { Width = "70", Type = new EnumValue<TableWidthUnitValues>(TableWidthUnitValues.Dxa) }
            },
            new TableGrid(
                            new GridColumn { Width = "454" },
                            new GridColumn { Width = "2222" },
                            new GridColumn { Width = "2738" },
                            new GridColumn { Width = "1287" },
                            new GridColumn { Width = "1701" },
                            new GridColumn { Width = "1077" },
                            new GridColumn { Width = "998" }
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
                AddSpacing(currentParagraph, 40, 40, spaceSimple);
                AddAligmentToParagrah(currentParagraph, ParagraphAligment.Centre);
                currentRun = (Run)headerRun.CloneNode(true);

                currentRun.Append(new Text(x));
                currentParagraph.Append(currentRun);

                currentCell = GetNewCell(currentParagraph, GetCellProperty(mauveCode, "506", TableRowAlignmentValues.Center, TableVerticalAlignmentValues.Center));
                headerRow.Append(currentCell);
            });

            tableResumeIntervention.Append(headerRow);

            docBody.Append(tableResumeIntervention);

            var mandatsByEmployeur = from data in utilisateur.Conseiller.Mandats.OrderByDescending(x => x.DateDebut)
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

                employeurParagraphModele = new Paragraph();
                AddSpacing(employeurParagraphModele, 40, 40, 0);

                employeurRunModele = new Run();
                employeurRunModele.Append(GetRunProperties("Arial Gras", "262626", "20", true, false));
                employeurRunModele.Append(new Text(employeur.Key.Nom));

                employeurParagraphModele.Append(employeurRunModele);
                currentCell.Append(employeurParagraphModele);
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
                        new KeyValuePair<string, string>($"{mandat.DateDebut.Year} - {mandat.DateFin.Year}", "1134"),
                        new KeyValuePair<string, string>(mandat.Efforts.ToString(), "1124")
                    }
                    .ToList().ForEach(x =>
                    {
                        mandatParagraphModele = new Paragraph();
                        AddSpacing(mandatParagraphModele, 40, 40, 0);
                        AddAligmentToParagrah(mandatParagraphModele, ParagraphAligment.Centre);

                        mandatRunModele = new Run();
                        mandatRunModele.Append(GetRunProperties("Arial", "Black", "18", false, false));
                        mandatRunModele.Append(new Text(x.Key));

                        currentCell = new TableCell();
                        currentCell.Append(GetCellProperty("White", x.Value, TableRowAlignmentValues.Center, TableVerticalAlignmentValues.Center));

                        mandatParagraphModele.Append(mandatRunModele);
                        currentCell.Append(mandatParagraphModele);

                        mandatRowModele.Append(currentCell);
                    });

                    tableResumeIntervention.Append(mandatRowModele);
                }
            }
        }

        public void CreateMandats(Body docBody)
        {
            Paragraph employeurParagraphModele, clientParagraphModele, currentParagraph;
            ParagraphProperties paragraphProperties;

            Run currentRun;

            Table tableInfoMandat;
            TableProperties tableProperties;
            TableRow tableRow;
            TableCell currentCell;


            docBody.Append(new Paragraph(new Run(new Break { Type = new EnumValue<BreakValues>(BreakValues.Page) })));

            var mandatsByEmployeur = from data in utilisateur.Conseiller.Mandats.OrderByDescending(x => x.DateDebut)
                                     group data by data.Projet.SocieteDeConseil.Nom into g
                                     select g;


            foreach (var employeur in mandatsByEmployeur)
            {
                employeurParagraphModele = new Paragraph();
                paragraphProperties = new ParagraphProperties();

                paragraphProperties.ParagraphStyleId = new ParagraphStyleId { Val = "Titre1" };
                employeurParagraphModele.Append(paragraphProperties);
                employeurParagraphModele.Append(new Run(new Text(employeur.Key)));

                docBody.Append(employeurParagraphModele);

                var mandatsByClientsOfAnEmployeur = from data in employeur
                                                    group data by data.Projet.Client.Nom into g
                                                    select g;

                foreach (var client in mandatsByClientsOfAnEmployeur)
                {
                    clientParagraphModele = new Paragraph();
                    paragraphProperties = new ParagraphProperties();
                    paragraphProperties.ParagraphStyleId = new ParagraphStyleId { Val = "Titre2" };
                    clientParagraphModele.Append(paragraphProperties);
                    clientParagraphModele.Append(new Run(new Text(client.Key)));

                    docBody.Append(clientParagraphModele);

                    foreach (var mandat in client)
                    {
                        tableInfoMandat = new Table();
                        tableProperties = new TableProperties();
                        tableProperties.Append(new TableWidth { Width = "10418", Type = new EnumValue<TableWidthUnitValues>(TableWidthUnitValues.Dxa) });
                        tableProperties.Append(new TableLayout { Type = new EnumValue<TableLayoutValues>(TableLayoutValues.Fixed) });
                        tableProperties.Append(new TableCellMargin
                            (
                                new LeftMargin { Width = "70", Type = new EnumValue<TableWidthUnitValues>(TableWidthUnitValues.Dxa) },
                                new RightMargin { Width = "70", Type = new EnumValue<TableWidthUnitValues>(TableWidthUnitValues.Dxa) }
                            ));

                        tableInfoMandat.Append(tableProperties);

                        TableGrid tableGrid = new TableGrid();
                        tableGrid.Append(new GridColumn { Width = "1600" });
                        tableGrid.Append(new GridColumn { Width = "8818" });
                        tableInfoMandat.Append(tableGrid);

                        TableLook tableLook = new TableLook
                        {
                            FirstRow = new OnOffValue(false),
                            LastRow = new OnOffValue(false),
                            FirstColumn = new OnOffValue(false),
                            LastColumn = new OnOffValue(false),
                            NoHorizontalBand = new OnOffValue(false),
                            NoVerticalBand = new OnOffValue(false)
                        };

                        tableInfoMandat.Append(tableLook);

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
                        docBody.Append(new Paragraph());

                        currentParagraph = new Paragraph();
                        paragraphProperties = new ParagraphProperties();
                        paragraphProperties.Append(new SpacingBetweenLines
                        {
                            After = "60",
                            Before = "60",
                            Line = "233"
                        });

                        paragraphProperties.Append(new Justification { Val = new EnumValue<JustificationValues>(JustificationValues.Both) });

                        currentParagraph.Append(paragraphProperties.CloneNode(true));

                        currentRun = new Run(GetRunProperties("Arial", "Black", "21", false, false));
                        currentRun.Append(new Text(mandat.Projet.Description));
                        currentParagraph.Append(currentRun);

                        docBody.Append(currentParagraph);


                        //Tâches

                        currentParagraph = new Paragraph();
                        currentParagraph.Append(paragraphProperties.CloneNode(true));

                        currentRun = new Run(GetRunProperties("Arial", "Black", "21", false, false));
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
                                Line = "233"
                            })
                            { ParagraphStyleId = new ParagraphStyleId { Val = "Puce1" } });

                            currentRun = new Run(GetRunProperties("Arial", "Black", "21", false, false));
                            currentRun.Append(new Text(tache.Description));
                            currentParagraph.Append(currentRun);
                            docBody.Append(currentParagraph);
                        }

                        //End tâches

                        if (mandat.Projet.Technologies != null)
                        {
                            currentParagraph = new Paragraph();
                            currentParagraph.Append(paragraphProperties.CloneNode(true));

                            currentRun = new Run(GetRunProperties("Arial", "Black", "21", false, false));
                            currentRun.Append(new Text("Environnement technologique: " + String.Join(',', mandat.Projet.Technologies.Select(x => x.Nom.ToUpper()))));
                            currentParagraph.Append(currentRun);

                            docBody.Append(currentParagraph);
                        }

                        docBody.Append(GetNewBlankParagraph(2));
                    }

                }

            }

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
