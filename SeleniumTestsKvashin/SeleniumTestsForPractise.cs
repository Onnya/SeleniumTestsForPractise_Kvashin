using System.Drawing;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

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

    [Test]
    public void EnableChristmasTheme()
    {
        // 1. Авторизируемся
        Authorize();
        
        // 2. Кликаем по иконке профиля
        var accountButton = _driver.FindElement(By.CssSelector("[data-tid='PopupMenu__caption']"));
        accountButton.Click();
        
        // 3. Кликаем по кнопке "Настройки"
        var settings = _driver.FindElement(By.CssSelector("[data-tid='Settings']"));
        settings.Click();
        
        // 4. Кликаем по переключателю темы
        var toggler = _driver.FindElement(By.ClassName("react-ui-toggle-handle"));
        toggler.Click();
        
        // 5. Кликаем по кнопке "Сохранить"
        var saveButton = _driver.FindElements(By.CssSelector("[type='button']"))
            .First(element=>element.FindElement(By.CssSelector("span")).Text == "Сохранить");
        saveButton.Click();
        
        // 6. Получаем куки новогодней темы
        var newYearCookie = _driver.Manage().Cookies.AllCookies.First(cookie=>cookie.Name.StartsWith("newYear"));
        
        // 7. Получаем элемент с id == root, где располагаются div с елями, div хедера и section с остальным содержимым
        var root = _driver.FindElement(By.CssSelector("#root"));
        
        // 8. Проверяем, что в доме 2 div (ели появились) и у куки новогодней темы значение == true
        using (new AssertionScope())
        {
            root.FindElements(By.XPath("./child::div")).Count.Should().Be(2);
            newYearCookie.Value.Should().Be("true");
        }
    }

    [Test]
    public void CancelProfileEditing()
    {
        // 1. Авторизируемся
        Authorize();
        
        // 2. Кликаем по иконке профиля
        var accountButton = _driver.FindElement(By.CssSelector("[data-tid='PopupMenu__caption']"));
        accountButton.Click();
        
        // 3. Кликаем по кнопке "Настройки"
        var profileEdit = _driver.FindElement(By.CssSelector("[data-tid='ProfileEdit']"));
        profileEdit.Click();
        
        // 4. Находим поле ввода ФИО
        var fio = _driver.FindElement(By.CssSelector("[data-tid='FIO']"));
        var fioInput = fio.FindElement(By.CssSelector("[data-tid='Input']"));
        
        // 5. Получаем текущее ФИО
        var fioText = fioInput.FindElement(By.CssSelector("[type='text']")).GetAttribute("value");
        
        // 6. Добавляем текст к текущему ФИО
        fioInput.SendKeys(" Иванович");
        
        // 7. Отменяем редактирование нажатием на кнопку "Отменить"
        var pageHeader = _driver.FindElement(By.CssSelector("[data-tid='PageHeader']"));
        var cancelButton = pageHeader.FindElements(By.CssSelector("button"))
            .First(element=>element.Text == "Отменить");
        cancelButton.Click();
        
        // 8. Получаем ФИО со страницы профиля
        var fioAfterCancelling = _driver.FindElement(By.CssSelector("[data-tid='EmployeeName']"));
        
        // 9. Проверяем, что ФИО не изменилось
        fioAfterCancelling.Text.Should().Be(fioText);
    }

    [Test]
    public void IsEventsCalendarDateToday()
    {
        // 1. Авторизируемся
        Authorize();
        
        // 2. Неявное ожидание окончания авторизации
        _driver.FindElement(By.CssSelector("[data-tid='Title']"));
        
        // 3. Переход на страницу мероприятий
        _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/events");
        
        // 4. Получение фрагментов даты из календаря
        var datePicker = _driver.FindElement(By.CssSelector("[data-tid='DatePicker']"));
        var dateByFragments = datePicker.FindElements(By.CssSelector("[data-fragment]"));
        
        // 5. Склеивание фрагментов даты из календаря в строку
        var eventsCalendarDate = string.Join(".", dateByFragments.Select(element=>element.Text));
        
        // 6. Получение сегодняшней даты
        var todayDate = DateTime.Today.ToString("dd.MM.yyyy");
        
        // 7. Проверка, что даты совпали
        eventsCalendarDate.Should().Be(todayDate);
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