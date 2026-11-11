using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
namespace SeleniumTests
{
    public class Tests
    {
        ChromeDriver driver;

        private const string AddAndRemoveElements = "https://the-internet.herokuapp.com/add_remove_elements/";

        private const string CheckboxesPage = "https://the-internet.herokuapp.com/checkboxes";

        private const string DropdownPage = "https://the-internet.herokuapp.com/dropdown";

        private const string InputsPage = "https://the-internet.herokuapp.com/inputs";

        private const string StatusCodesPage = "https://the-internet.herokuapp.com/status_codes";

        private const string DragAndDropPage = "https://the-internet.herokuapp.com/drag_and_drop";

        private const string ShiftingContentPage = "https://the-internet.herokuapp.com/shifting_content";

        private const string GeolocationPage = "https://the-internet.herokuapp.com/geolocation";

        private const string JavaScriptErrorsPage = "https://the-internet.herokuapp.com/javascript_error";

        private const string ExitIntentPage = "https://the-internet.herokuapp.com/exit_intent";

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
        }

        [Test]
        public void AddAndDeleteButtons()
        {
            driver.Navigate().GoToUrl(AddAndRemoveElements);

            driver.FindElement(By.CssSelector("button[onclick='addElement()']")).Click();
            var countOfButtons = driver.FindElements(By.CssSelector(".added-manually")).Count();
            Assert.That(countOfButtons, Is.EqualTo(1));

            driver.FindElement(By.CssSelector(".added-manually")).Click();
            countOfButtons = driver.FindElements(By.CssSelector(".added-manually")).Count();
            Assert.That(countOfButtons, Is.EqualTo(0));
        }

        [Test]
        public void Checkbox()
        {
            driver.Navigate().GoToUrl(CheckboxesPage);

            var checkboxes = driver.FindElements(By.CssSelector("input[type='checkbox']"));
            var checkboxOne = checkboxes.First();

            if (!checkboxOne.Selected)
            {
                checkboxOne.Click();
            }

            Assert.That(checkboxOne.Selected, Is.True);
        }

        [Test]
        public void Dropdown()
        {
            driver.Navigate().GoToUrl(DropdownPage);

            var optionSelect = new SelectElement(driver.FindElement(By.Id("dropdown")));
            optionSelect.SelectByText("Option 1");

            Assert.That(optionSelect.SelectedOption.Text, Is.EqualTo("Option 1"));
        }

        [Test]
        public void Input()
        {
            driver.Navigate().GoToUrl(InputsPage);
            var input = driver.FindElement(By.CssSelector("input[type='number']"));
            input.Clear();
            input.SendKeys("1");

            Assert.That(input.GetAttribute("value"), Is.EqualTo("1"));
        }

        [Test]
        [TestCase("200")]
        [TestCase("301")]
        [TestCase("404")]
        [TestCase("500")]
        public void StatusCodes(string linkTextForStatusCodes)
        {
            driver.Navigate().GoToUrl(StatusCodesPage);
            driver.FindElement(By.LinkText(linkTextForStatusCodes)).Click();

            var paragraph = driver.FindElement(By.TagName("p"));
            var expectedPageText = $"This page returned a {linkTextForStatusCodes} status code.";

            Assert.That(paragraph.Text, Does.Contain(expectedPageText));
        }

        [Test]
        public void DragAndDropBlocks()
        {
            driver.Navigate().GoToUrl(DragAndDropPage);

            var leftColumn = driver.FindElement(By.Id("column-a"));
            var leftColumnHeader = leftColumn.Text;

            var rightColumn = driver.FindElement(By.Id("column-b"));
            var rightColumnHeader = rightColumn.Text;

            var actions = new Actions(driver);
            actions.DragAndDrop(leftColumn, rightColumn).Perform();

            Assert.That(leftColumn.Text, Is.EqualTo(rightColumnHeader));
            Assert.That(rightColumn.Text, Is.EqualTo(leftColumnHeader));
        }

        #region ShiftingContentPage

        [Test]
        public void ShiftingContent_ShiftImageByDefault()
        {
            driver.Navigate().GoToUrl(ShiftingContentPage);
            driver.FindElement(By.LinkText("Example 2: An image")).Click();

            var defaultShiftValue = -24;
            var defaultCssLeftValue = "0px";
            var expectedCssLeftValue = "-25px";

            var image = driver.FindElement(By.CssSelector("img.shift"));
            var startLocation = image.Location;
            var expectedLocation = startLocation;
            expectedLocation.X += defaultShiftValue;

            Assert.That(image.GetCssValue("left"), Is.EqualTo(defaultCssLeftValue));

            driver.Navigate().Refresh();

            image = driver.FindElement(By.CssSelector("img.shift"));

            var newCssLeftValue = image.GetCssValue("left");
            Assert.That(newCssLeftValue, Is.EqualTo(expectedCssLeftValue));

            var newLocation = image.Location;

            Assert.That(newLocation, Is.EqualTo(expectedLocation));
        }

        [Test]
        public void ShiftingContent_ShiftImageByQueryParameterModeRandom()
        {
            driver.Navigate().GoToUrl(ShiftingContentPage);
            driver.FindElement(By.LinkText("Example 2: An image")).Click();

            var modeRandomShift = driver.FindElement(By.CssSelector("a[href='/shifting_content/image?mode=random']"));

            var modeRandomShiftValue = -24;
            var defaultCssLeftValue = "0px";
            var expectedCssLeftValue = "-25px";

            var image = driver.FindElement(By.CssSelector("img.shift"));
            var startLocation = image.Location;
            var expectedLocation = startLocation;
            expectedLocation.X += modeRandomShiftValue;

            Assert.That(image.GetCssValue("left"), Is.EqualTo(defaultCssLeftValue));

            modeRandomShift.Click();

            image = driver.FindElement(By.CssSelector("img.shift"));

            var newCssLeftValue = image.GetCssValue("left");
            Assert.That(newCssLeftValue, Is.AnyOf(defaultCssLeftValue, expectedCssLeftValue));

            var newLocation = image.Location;

            Assert.That(newLocation, Is.AnyOf(startLocation, expectedLocation));
        }

        [Test]
        public void ShiftingContent_ShiftImageByQueryParameterPixelShiftHundred()
        {
            driver.Navigate().GoToUrl(ShiftingContentPage);
            driver.FindElement(By.LinkText("Example 2: An image")).Click();

            var pixelShift = driver.FindElement(By.CssSelector("a[href='/shifting_content/image?pixel_shift=100']"));

            var pixelShiftValue = -100;
            var defaultCssLeftValue = "0px";
            var expectedCssLeftValue = "-100px";

            var image = driver.FindElement(By.CssSelector("img.shift"));
            var startLocation = image.Location;
            var expectedLocation = startLocation;
            expectedLocation.X += pixelShiftValue;

            Assert.That(image.GetCssValue("left"), Is.EqualTo(defaultCssLeftValue));

            pixelShift.Click();

            image = driver.FindElement(By.CssSelector("img.shift"));

            var newCssLeftValue = image.GetCssValue("left");
            Assert.That(newCssLeftValue, Is.EqualTo(expectedCssLeftValue));

            var newLocation = image.Location;

            Assert.That(newLocation, Is.EqualTo(expectedLocation));
        }

        [Test]
        public void ShiftingContent_ShiftImageByQueryParameterImageTypeSimple()
        {
            driver.Navigate().GoToUrl(ShiftingContentPage);
            driver.FindElement(By.LinkText("Example 2: An image")).Click();

            var imageTypeSimple = driver.FindElement(By.CssSelector("a[href='/shifting_content/image?image_type=simple']"));

            var imageTypeSimpleShiftValue = -24;
            var defaultCssLeftValue = "0px";
            var expectedCssLeftValue = "-25px";

            var image = driver.FindElement(By.CssSelector("img.shift"));
            var startLocation = image.Location;
            var expectedLocation = startLocation;
            expectedLocation.X += imageTypeSimpleShiftValue;

            Assert.That(image.GetCssValue("left"), Is.EqualTo(defaultCssLeftValue));

            imageTypeSimple.Click();

            image = driver.FindElement(By.CssSelector("img.shift"));

            var newCssLeftValue = image.GetCssValue("left");
            Assert.That(newCssLeftValue, Is.EqualTo(expectedCssLeftValue));

            var newLocation = image.Location;

            Assert.That(newLocation, Is.EqualTo(expectedLocation));
        }

        #endregion

        [Test]
        public void Geolocation_ShowCurrentLocation()
        {
            driver.Navigate().GoToUrl(GeolocationPage);

            var expectedLatitude = 20.1;
            var expectedLongitude = 10.1;
            Dictionary<string, object?> expectedCoordinates = new Dictionary<string, object?>()
            {
                ["latitude"] = expectedLatitude,
                ["longitude"] = expectedLongitude,
                ["accuracy"] = 10
            };
            Dictionary<string, object?> geolocationPermissions = new Dictionary<string, object?>()
            {
                ["origin"] = "https://the-internet.herokuapp.com",
                ["permissions"] = new[] { "geolocation" }
            };

            driver.ExecuteCdpCommand("Emulation.setGeolocationOverride", expectedCoordinates);

            driver.ExecuteCdpCommand("Browser.grantPermissions", geolocationPermissions);

            var button = driver.FindElement(By.TagName("button"));
            button.Click();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));


            wait.Until(d => !string.IsNullOrEmpty(d.FindElement(By.Id("lat-value")).Text)
                            && !string.IsNullOrEmpty(d.FindElement(By.Id("long-value")).Text));

            var latitudeDiv = driver.FindElement(By.Id("lat-value"));
            var actualLatitude = Convert.ToDouble(latitudeDiv.Text, CultureInfo.InvariantCulture);

            var longitudeDiv = driver.FindElement(By.Id("long-value"));
            var actualLongitude = Convert.ToDouble(longitudeDiv.Text, CultureInfo.InvariantCulture);

            Assert.That(actualLatitude, Is.EqualTo(expectedLatitude));
            Assert.That(actualLongitude, Is.EqualTo(expectedLongitude));
        }

        [Test]
        public void JavaScriptErrors_CheckPageTextAndConsoleError()
        {
            driver.Navigate().GoToUrl(JavaScriptErrorsPage);

            var expectedText = "This page has a JavaScript error in the onload event. This is often a problem to using normal Javascript injection techniques.";
            var actualText = driver.FindElement(By.TagName("body")).Text;

            Assert.That(actualText, Is.EqualTo(expectedText));

            var logs = driver.Manage().Logs.GetLog(LogType.Browser);

            Assert.That(logs.Any(log => log.Level == LogLevel.Severe), Is.True);

            var error = logs.FirstOrDefault(log => log.Level == LogLevel.Severe);
            Assert.That(error!.Message, Does.Contain("reading 'xyz'"));
        }

        [Test]
        public void ExitIntent()
        {
            driver.Navigate().GoToUrl(ExitIntentPage);

            var expectedExitIntentText = "THIS IS A MODAL WINDOW";

            ((IJavaScriptExecutor)driver).ExecuteScript(
                @"document.documentElement.dispatchEvent( new MouseEvent('mouseleave', 
                    {
                        bubbles: false,
                        clientY: 0,
                        clientX: 10
                    })
                );
            ");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElement(By.Id("ouibounce-modal")).GetCssValue("display") == "block");

            Thread.Sleep(1000);
            var exitIntent = driver.FindElement(By.Id("ouibounce-modal"));
            Assert.That(exitIntent.Text, Does.Contain(expectedExitIntentText));

            var closeIntentDiv = driver.FindElement(By.CssSelector("div[class='modal-footer']"));
            closeIntentDiv.Click();

            Thread.Sleep(1000);
            exitIntent = driver.FindElement(By.Id("ouibounce-modal"));

            Assert.That(exitIntent.GetCssValue("display"), Is.EqualTo("none"));
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }
    }
}


