using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NUnitTestProjectWithSelenium
{
    public class OvouTest
    {
        static IWebDriver driver;

        /// <summary>
        /// This is initialize the driver and start the crome browser
        /// </summary>
        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMinutes(1);
            
            driver.Navigate().GoToUrl("https://test.ovou.me/");
            
            Thread.Sleep(4000);
        }

        /// <summary>
        /// This is to check login with invalid emaid id and check the error msg
        /// </summary>
        [Test, Order(1)]
        public void CheckLoginWithInvalidEmail()
        {
            try
            {
                //Login
                IWebElement ele = driver.FindElement(By.Id("login-email"));
                ele.SendKeys("aaaaa");

                Thread.Sleep(2000);

                driver.FindElement(By.XPath("//body")).Click();

                Thread.Sleep(2000);


                IWebElement errorDiv = driver.FindElement(By.Id("login-email-feedback"));
                string errorMsg = errorDiv.Text;

                Assert.AreEqual("Email format is wrong.", errorMsg);
                driver.Close();

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// This is to check login button is getting enabled or not after entering creds
        /// </summary>
        [Test, Order(2)]
        public void CheckLoginButtonAfterEnteringCreds()
        {
            try
            {
                IWebElement ele = driver.FindElement(By.Id("login-email"));
                ele.SendKeys("hsk031995@gmail.com");

                IWebElement ele1 = driver.FindElement(By.Id("login-password"));
                ele1.SendKeys("test@1234");

                Thread.Sleep(2000);

                IWebElement loginButton = driver.FindElement(By.CssSelector("button[class='chakra-button css-bzjc2j']"));
                string attr = loginButton.GetAttribute("disabled");
                Assert.AreNotEqual("disabled", attr);
                driver.Close();

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// This is to check login with 3 char password and check the error msg
        /// </summary>
        [Test, Order(3)]
        public void CheckLoginWithThreeCharPassword()
        {
            try
            {
                IWebElement ele = driver.FindElement(By.Id("login-email"));
                ele.SendKeys("hsk031995@gmail.com");

                IWebElement ele1 = driver.FindElement(By.Id("login-password"));
                ele1.SendKeys("test");

                Thread.Sleep(2000);
                ele.Click();

                Thread.Sleep(2000);

                IWebElement passwordError = driver.FindElement(By.Id("login-password-feedback"));
                string error = passwordError.Text;
                Assert.AreEqual(error, "Password at least must be 8 characters.");
                driver.Close();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// This is to check login with invalid password and check the error msg
        /// </summary>
        [Test, Order(4)]
        public void CheckLoginWithInvalidPassword()
        {
            try
            {
                IWebElement ele = driver.FindElement(By.Id("login-email"));
                ele.SendKeys("hsk031995@gmail.com");

                IWebElement ele1 = driver.FindElement(By.Id("login-password"));
                ele1.SendKeys("000000000");

                IWebElement ele2 = driver.FindElement(By.CssSelector("button[class='chakra-button css-bzjc2j']"));
                ele2.Click();

                Thread.Sleep(3000);

                var element = driver.FindElements(By.CssSelector("div[class='css-1rr4qq7']")).Count >= 1 ? driver.FindElement(By.CssSelector("div[class='css-1rr4qq7']")) : null;
                Assert.IsNotNull(element);
                driver.Close();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// This is to add/edit about text and validate same on home page
        /// </summary>
        [Test, Order(5)]
        public void EditAndValidateAbout()
        {
            try
            {
                DoLogin();

                Thread.Sleep(5000);
                IWebElement ele = driver.FindElement(By.CssSelector("a[href='/o/my-profile/edit-bio']"));
                ele.Click();
                Thread.Sleep(3000);
                WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 3));
                IWebElement eleAboutText = wait.Until(ExpectedConditions.ElementExists(By.Id("card-details-bio")));
                string aboutText = "Test About By Selenium";
                eleAboutText.Clear();
                eleAboutText.SendKeys(aboutText);

                Thread.Sleep(3000);

                IWebElement eleSave = driver.FindElement(By.CssSelector("button[class='chakra-button css-bzjc2j']"));
                eleSave.Click();

                IWebElement eleAboutLabel = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div[class='css-bp6jx9']")));
                Assert.IsTrue(eleAboutLabel.Text.Contains(aboutText));

                driver.Close();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// This is to add/edit links and validate same on home page
        /// </summary>
        [Test, Order(6)]
        public void AddAndValidateLinks()
        {
            try
            {
                DoLogin();

                Thread.Sleep(5000);


                bool isAdd = IsElementPresent(By.CssSelector("a[href='/o/my-profile/edit-profile-links?action=add']"));
                IWebElement ele = null;
                if (isAdd)
                {
                    ele = driver.FindElement(By.CssSelector("a[href='/o/my-profile/edit-profile-links?action=add']"));
                    ele.Click();
                }
                else
                {
                    ele = driver.FindElement(By.CssSelector("a[href='/o/my-profile/edit-profile-links?action=edit']"));
                    ele.Click();
                    List<IWebElement> eles = driver.FindElements(By.CssSelector("button[class='chakra-button css-gocinq']")).ToList();
                    foreach (IWebElement eleDelete in eles)
                    {
                        if(eleDelete.Text == "Delete")
                        {
                            eleDelete.Click();
                            Thread.Sleep(1000);
                        }
                    }
                    Thread.Sleep(2000);

                    WebDriverWait waitAdd = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
                    IWebElement eleAddLink = waitAdd.Until(ExpectedConditions.ElementExists(By.CssSelector("div[class='chakra-stack css-1glv4r3']")));
                    eleAddLink.Click();
                }

                
                Thread.Sleep(3000);
                WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 3));
                IWebElement eleTitleText = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[name='profileLink.title']")));

                eleTitleText.Click();
                IWebElement eleURL= wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[name='profileLink.link']")));

                eleURL.Click();

                driver.FindElement(By.XPath("//body")).Click();
                Thread.Sleep(2000);

                List<IWebElement> eleErrors = driver.FindElements(By.CssSelector("div[class='chakra-form__error-message css-8dhjh4']")).ToList();
                
                Assert.AreEqual(2, eleErrors.Count());

                Assert.AreEqual("Title is required.", eleErrors[0].Text);
                Assert.AreEqual("Link is required.", eleErrors[1].Text);


                string titleText = "Test Title By Selenium";
                string url = "https://test.ovou.me/";

                eleTitleText.SendKeys(titleText);
                eleURL.SendKeys(url);

                Thread.Sleep(3000);

                IWebElement eleSave = driver.FindElement(By.CssSelector("button[class='chakra-button css-bzjc2j']"));
                eleSave.Click();

                IWebElement eleLink = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("a[href='"+ url +"']")));
                Assert.AreEqual(url, eleLink.GetAttribute("href"));

                driver.Close();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// This is to delete the links and check same on home page
        /// </summary>
        [Test, Order(7)]
        public void DeleteAndAddLinks()
        {
            try
            {
                DoLogin();

                Thread.Sleep(5000);


                bool isEdit = IsElementPresent(By.CssSelector("a[href='/o/my-profile/edit-profile-links?action=edit']"));
                Assert.IsTrue(isEdit);
                IWebElement ele = driver.FindElement(By.CssSelector("a[href='/o/my-profile/edit-profile-links?action=edit']"));
                ele.Click();
                List<IWebElement> eles = driver.FindElements(By.CssSelector("span[class='chakra-text css-iwcnlk']")).ToList();
                foreach (IWebElement eleDelete in eles)
                {
                    if (eleDelete.Text == "Delete")
                    {
                        eleDelete.Click();
                        Thread.Sleep(1000);
                    }
                }
                Thread.Sleep(2000);

                WebDriverWait waitAdd = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
                IWebElement eleAddLink = waitAdd.Until(ExpectedConditions.ElementExists(By.CssSelector("div[class='chakra-stack css-1glv4r3']")));
                eleAddLink.Click();


                Thread.Sleep(3000);
                WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 3));
                IWebElement eleTitleText = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[name='profileLink.title']")));

                eleTitleText.Click();
                IWebElement eleURL = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[name='profileLink.link']")));

                eleURL.Click();

                driver.FindElement(By.XPath("//body")).Click();
                Thread.Sleep(2000);

                List<IWebElement> eleErrors = driver.FindElements(By.CssSelector("div[class='chakra-form__error-message css-8dhjh4']")).ToList();

                Assert.AreEqual(2, eleErrors.Count());

                Assert.AreEqual("Title is required.", eleErrors[0].Text);
                Assert.AreEqual("Link is required.", eleErrors[1].Text);


                string titleText = "Test Title By Selenium";
                string url = "https://test.ovou.me/";

                eleTitleText.SendKeys(titleText);
                eleURL.SendKeys(url);

                Thread.Sleep(3000);

                IWebElement eleSave = driver.FindElement(By.CssSelector("button[class='chakra-button css-bzjc2j']"));
                eleSave.Click();

                IWebElement eleLink = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("a[href='" + url + "']")));
                Assert.AreEqual(url, eleLink.GetAttribute("href"));

                driver.Close();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// This is to add/edit/delete contact info and validate same on home screen
        /// </summary>
        [Test, Order(8)]
        public void EditAndValidateContactInfo()
        {
            try
            {
                DoLogin();

                Thread.Sleep(3000);
                IWebElement eleEdit = driver.FindElement(By.CssSelector("a[href='/o/my-profile/edit-contact-info']"));
                eleEdit.Click();
                Thread.Sleep(3000);

                WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
                IWebElement eleDeleteButtonCheck = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[class='chakra-button css-gocinq']")));
                Thread.Sleep(1000);
                List<IWebElement> eles = driver.FindElements(By.CssSelector("button[class='chakra-button css-gocinq']")).ToList();
                foreach (IWebElement ele in eles)
                {
                    ele.Click();
                }
                Thread.Sleep(3000);

                string phone = string.Empty, email = string.Empty, website = string.Empty, location = string.Empty;

                List<IWebElement> eleSpans = driver.FindElements(By.CssSelector("span[class='chakra-text css-q2y3yl']")).ToList();
                foreach (IWebElement ele in eleSpans)
                {
                    ele.Click();
                    Thread.Sleep(2000);

                    if (ele.Text == "Add phone")
                    {
                        IWebElement elePhoneTextbox = driver.FindElement(By.CssSelector("input[placeholder='Phone']"));
                        phone = "1111111111";
                        elePhoneTextbox.SendKeys(phone);
                        
                    }
                    else if (ele.Text == "Add email")
                    {
                        IWebElement eleEmailTextbox = driver.FindElement(By.CssSelector("input[name='emails.0.value']"));
                        eleEmailTextbox.SendKeys("xxxx");
                        Thread.Sleep(2000);

                        driver.FindElement(By.XPath("//body")).Click();
                        Thread.Sleep(2000);
                        IWebElement eleEmailError = driver.FindElement(By.CssSelector("div[class='chakra-form__error-message css-8dhjh4']"));
                        Assert.AreEqual("Email format is wrong.", eleEmailError.Text);

                        List<IWebElement> elesDelete = driver.FindElements(By.CssSelector("button[class='chakra-button css-gocinq']")).ToList();
                        elesDelete[1].Click();
                        Thread.Sleep(1000);
                        ele.Click();
                        Thread.Sleep(1000);
                        eleEmailTextbox = driver.FindElement(By.CssSelector("input[name='emails.0.value']"));
                        email = "amol007amol@gmail.com";
                        eleEmailTextbox.SendKeys(email);


                    }
                    else if (ele.Text == "Add website")
                    {
                        IWebElement eleWebsiteTextbox = driver.FindElement(By.CssSelector("input[name='websites.0.value']"));
                        eleWebsiteTextbox.SendKeys("xxxx");
                        Thread.Sleep(2000);

                        driver.FindElement(By.XPath("//body")).Click();

                        IWebElement eleWebsiteError = driver.FindElement(By.CssSelector("div[class='chakra-form__error-message css-8dhjh4']"));
                        Assert.AreEqual("Website format is wrong.", eleWebsiteError.Text);

                        eleWebsiteTextbox.Clear();
                        Thread.Sleep(1000);
                        //ele.Click();
                        //Thread.Sleep(1000);
                        List<IWebElement> elesDelete = driver.FindElements(By.CssSelector("button[class='chakra-button css-gocinq']")).ToList();
                        elesDelete[2].Click();
                        Thread.Sleep(1000);
                        ele.Click();
                        Thread.Sleep(1000);
                        eleWebsiteTextbox = driver.FindElement(By.CssSelector("input[name='websites.0.value']"));
                        website = "www.test.ovou.me";
                        eleWebsiteTextbox.SendKeys(website);
                    }
                    else if (ele.Text == "Add location")
                    {
                        IWebElement eleCityTextbox = driver.FindElement(By.CssSelector("input[name='locations.0.city']"));
                        eleCityTextbox.SendKeys("Pune");
                        Thread.Sleep(1000);
                        IWebElement eleStateTextbox = driver.FindElement(By.CssSelector("input[name='locations.0.state']"));
                        eleStateTextbox.SendKeys("Maharashtra");
                        Thread.Sleep(1000);
                        IWebElement eleCountryTextbox = driver.FindElement(By.CssSelector("input[name='locations.0.country']"));
                        eleCountryTextbox.SendKeys("India");
                        Thread.Sleep(1000);
                        location = "Location: Pune Maharashtra India";
                    }
                }

                IWebElement eleButton = driver.FindElement(By.CssSelector("button[class='chakra-button css-bzjc2j']"));
                eleButton.Click();
                Thread.Sleep(5000);
                List<IWebElement> eleSpanLabels = driver.FindElements(By.CssSelector("span[class='chakra-text css-f3dzr7']")).ToList();

                Assert.AreEqual("+1 1111111111", eleSpanLabels[eleSpanLabels.Count() -4 ].Text);
                Assert.AreEqual(email, eleSpanLabels[eleSpanLabels.Count() - 3].Text);
                Assert.AreEqual(website, eleSpanLabels[eleSpanLabels.Count() - 2].Text);
                Assert.AreEqual(location, eleSpanLabels[eleSpanLabels.Count() - 1].Text);
                
                driver.Close();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// This is helper method to do login
        /// </summary>
        public void DoLogin()
        {
            try
            {
                //Login
                IWebElement ele = driver.FindElement(By.Id("login-email"));
                ele.SendKeys("hsk031995@gmail.com");

                IWebElement ele1 = driver.FindElement(By.Id("login-password"));
                ele1.SendKeys("test@1234");

                Thread.Sleep(2000);

                IWebElement ele2 = driver.FindElement(By.CssSelector("button[class='chakra-button css-bzjc2j']"));
                ele2.Click();

                Thread.Sleep(2000);

                WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
                IWebElement eleAbout = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("p[class='chakra-text css-znd25t']")));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// This is helper method to check if element is rendered/present or not on browser
        /// </summary>
        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}