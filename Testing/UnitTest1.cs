using System;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace Genspil.Tests
{
    // Hvis Program-klassen er internal, skal du s�rge for at tilf�je
    // [assembly: InternalsVisibleTo("Genspil.Tests")] i AssemblyInfo.cs i hovedprojektet.
    public class ProgramTests
    {
        [Fact]
        public void ShowMenu_ExitsOnOption3()
        {
            // Arrange: Simulerer input "3" for at v�lge "Afslut" i hovedmenuen.
            string simulatedInput = "3" + Environment.NewLine;
            using (var sr = new StringReader(simulatedInput))
            {
                Console.SetIn(sr);
                using (var sw = new StringWriter())
                {
                    Console.SetOut(sw);

                    // Act: Kald menuen
                    Program.ShowMenu();

                    // Assert: Output skal indeholde menuen med "Afslut"
                    string output = sw.ToString();
                    Assert.Contains("Afslut", output);
                }
            }
        }

        [Fact]
        public void ShowInventoryMenu_ExitsOnOption6()
        {
            // Arrange: Simulerer input "6" for at afslutte lagerliste-menuen.
            string simulatedInput = "6" + Environment.NewLine;
            using (var sr = new StringReader(simulatedInput))
            {
                Console.SetIn(sr);
                using (var sw = new StringWriter())
                {
                    Console.SetOut(sw);

                    // Act: Kald lagerliste-menuen
                    Program.ShowInventoryMenu();

                    // Assert: Output skal indeholde teksten "Lagerliste Menu"
                    string output = sw.ToString();
                    Assert.Contains("Lagerliste Menu", output);
                }
            }
        }

        [Fact]
        public void ShowRequestMenu_ExitsOnOption4()
        {
            // Arrange: Simulerer input "4" for at afslutte foresp�rgsels-menuen.
            string simulatedInput = "4" + Environment.NewLine;
            using (var sr = new StringReader(simulatedInput))
            {
                Console.SetIn(sr);
                using (var sw = new StringWriter())
                {
                    Console.SetOut(sw);

                    // Act: Kald foresp�rgsels-menuen
                    Program.ShowRequestMenu();

                    // Assert: Output skal indeholde teksten "Foresp�rgsler Menu"
                    string output = sw.ToString();
                    Assert.Contains("Foresp�rgsler Menu", output);
                }
            }
        }
    }
}