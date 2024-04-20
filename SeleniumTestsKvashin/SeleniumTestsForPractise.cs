using System.Drawing;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SeleniumTestsKvashin;

public class SeleniumTestsForPractise
{
    private ChromeDriver _driver = null!;

    private void Authorize()
    {
        _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");
        
        // 1. Вводим логин
        var login = _driver.FindElement(By.Id("Username"));
        login.SendKeys("timur.kvashin@urfu.me");
        
        // 2. Вводим пароль
        var password = _driver.FindElement(By.Id("Password"));
        password.SendKeys("Basketbol-12");
        
        // 3. Нажимаем кнопку входа
        var enter = _driver.FindElement(By.Name("button"));
        enter.Click();
    }
    
    [Test]
    public void Authorization()
    {
        // 1. Авторизируемся
        Authorize();
        
        // 2. Неявное ожидание страницы новостей
        var newsTitle = _driver.FindElement(By.CssSelector("[data-tid='Title']"));
        
        // 3. Получаем Url текущей страницы
        var currentUrl = _driver.Url;
        
        // 4. Проверяем, что заголовок и Url совпадают с заголовком и Url страницы новостей
        using (new AssertionScope())
        {
            newsTitle.Text.Should().Be("Новости");
            currentUrl.Should().Be("https://staff-testing.testkontur.ru/news");
        }
    }

    [Test]
    public void SideMenuClickToCommunities()
    {
        // 1. Авторизируемся
        Authorize();
        
        // 2. Уменьшаем размер окна
        _driver.Manage().Window.Size = new Size(1000, 700);
        
        // 3. Кликаем на боковое меню
        var sideMenu = _driver.FindElement(By.CssSelector("[data-tid='SidebarMenuButton']"));
        sideMenu.Click();
        
        // 4. Кликаем на "Сообщества"
        var sidePageBody = _driver.FindElement(By.CssSelector("[data-tid='SidePageBody']"));
        var community = sidePageBody.FindElement(By.CssSelector("[data-tid='Community']"));
        community.Click();
        
        // 5. Неявное ожидание страницы сообществ
        var communityTitle = _driver.FindElement(By.CssSelector("[data-tid='Title']"));
        
        // 6. Получаем Url текущей страницы
        var currentUrl = _driver.Url;
        
        // 7. Проверяем, что заголовок и Url совпадают с заголовком и Url страницы сообществ
        using (new AssertionScope())
        {
            communityTitle.Text.Should().Be("Сообщества");
            currentUrl.Should().Be("https://staff-testing.testkontur.ru/communities");
        }
        
    }
    
    [SetUp]
    public void SetUp()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");
        
        _driver = new ChromeDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
    }

    [TearDown]
    public void TearDown()
    {
        _driver.Close();
        _driver.Quit();
    }
}