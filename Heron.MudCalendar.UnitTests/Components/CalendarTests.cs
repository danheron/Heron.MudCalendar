using System;
using FluentAssertions;
using Heron.MudCalendar.UnitTests.Viewer.TestComponents.Calendar;
using MudBlazor;
using MudBlazor.Extensions;

namespace Heron.MudCalendar.UnitTests.Components;

[SetCulture("en-GB")]
public class CalendarTests : BunitTest
{
    [Test]
    public void SimpleTest()
    {
        var cut = Context.RenderComponent<CalendarTest>();

        cut.FindAll("div.mud-calendar").Count.Should().Be(1);
    }

    [Test]
    public void ChangeViews()
    {
        var cut = Context.RenderComponent<CalendarTest>();
        cut.FindAll("div.mud-cal-month-view").Count.Should().Be(1);

        var comp = cut.FindComponent<EnumSwitch<CalendarView>>();
        var buttons = comp.FindAll("button");
        Assert.That(4, Is.EqualTo(buttons.Count));
        
        buttons[1].Click();
        cut.FindAll("div.mud-cal-week-view").Count.Should().Be(1);

        buttons = comp.FindAll("button");
        buttons[2].Click();
        cut.FindAll("div.mud-cal-week-view").Count.Should().Be(1);
    }

    [Test]
    public void RestrictViews()
    {
        var cut = Context.RenderComponent<CalendarTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();

        comp.SetParam(x => x.ShowDay, false);
        comp.SetParam(x => x.ShowWeek, false);
        comp.SetParam(x => x.ShowWorkWeek, false);

        comp.FindComponent<EnumSwitch<CalendarView>>().Find("div").ClassList.Should().Contain("d-none");
        cut.FindAll("div.mud-cal-month-view").Count.Should().Be(1);
    }

    [Test]
    public void EnsureViewIsChangedWhenCurrentViewNotAllowed()
    {
        var cut = Context.RenderComponent<CalendarTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();

        comp.SetParam(x => x.ShowMonth, false);

        cut.FindAll("div.mud-cal-month-view").Count.Should().Be(0);
        cut.FindAll("div.mud-cal-week-view").Count.Should().Be(1);
    }

    [Test]
    public void NextButton()
    {
        var cut = Context.RenderComponent<CalendarTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        comp.SetParam(x => x.FirstDayOfWeek!, DayOfWeek.Monday);
        comp.SetParam(x => x.FirstDayOfWorkWeek!, DayOfWeek.Sunday);

        // Month View
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 1));
        comp.FindAll("button.mud-icon-button")[1].Click();
        comp.FindAll("div.mud-cal-month-cell-title")[0].TextContent.Should().Be("30");
        comp.Instance.CurrentDateRange.Should().NotBeNull();
        comp.Instance.CurrentDateRange!.Start.Should().Be(new DateTime(2023, 1, 30));
        comp.Instance.CurrentDateRange.End.Should().Be(new DateTime(2023, 3, 5));
        
        // Week View
        comp.SetParam(x => x.View, CalendarView.Week);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 2));
        comp.FindAll("button.mud-icon-button")[1].Click();
        var day = comp.FindAll("div.mud-cal-grid.mud-cal-grid-header.mud-cal-week-header div")[1].TextContent;
        day.Substring(day.Length - 1, 1).Should().Be("9");
        comp.Instance.CurrentDateRange.Should().NotBeNull();
        comp.Instance.CurrentDateRange!.Start.Should().Be(new DateTime(2023, 1, 9));
        comp.Instance.CurrentDateRange.End.Should().Be(new DateTime(2023, 1, 15));

        // Work Week View
        comp.SetParam(x => x.View, CalendarView.WorkWeek);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 2));
        comp.FindAll("button.mud-icon-button")[1].Click();
        day = comp.FindAll("div.mud-cal-grid.mud-cal-grid-header.mud-cal-work-week-header div")[1].TextContent;
        day.Substring(day.Length - 1, 1).Should().Be("8");
        comp.Instance.CurrentDateRange.Should().NotBeNull();
        comp.Instance.CurrentDateRange!.Start.Should().Be(new DateTime(2023, 1, 8));
        comp.Instance.CurrentDateRange.End.Should().Be(new DateTime(2023, 1, 12));

        // Day View
        comp.SetParam(x => x.View, CalendarView.Day);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 4));
        comp.FindAll("button.mud-icon-button")[1].Click();
        day = comp.FindAll("div.mud-cal-grid.mud-cal-grid-header.mud-cal-day-header div")[1].TextContent;
        day.Substring(day.Length - 1, 1).Should().Be("5");
        comp.Instance.CurrentDateRange.Should().NotBeNull();
        comp.Instance.CurrentDateRange!.Start.Should().Be(new DateTime(2023, 1, 5));
        comp.Instance.CurrentDateRange.End.Should().Be(new DateTime(2023, 1, 5));
    }

    [Test]
    public void PrevButton()
    {
        var cut = Context.RenderComponent<CalendarTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        comp.SetParam(x => x.FirstDayOfWeek!, DayOfWeek.Monday);
        comp.SetParam(x => x.FirstDayOfWorkWeek!, DayOfWeek.Sunday);

        // Month View
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 1));
        comp.FindAll("button.mud-icon-button")[0].Click();
        comp.Find("div.mud-cal-month-cell-title").TextContent.Should().Be("28");
        comp.Instance.CurrentDateRange.Should().NotBeNull();
        comp.Instance.CurrentDateRange!.Start.Should().Be(new DateTime(2022, 11, 28));
        comp.Instance.CurrentDateRange.End.Should().Be(new DateTime(2023, 1, 1));

        // Week View
        comp.SetParam(x => x.View, CalendarView.Week);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 14));
        comp.FindAll("button.mud-icon-button")[0].Click();
        var day = comp.FindAll("div.mud-cal-grid.mud-cal-grid-header.mud-cal-week-header div")[1].TextContent;
        day.Substring(day.Length - 1, 1).Should().Be("2");
        comp.Instance.CurrentDateRange.Should().NotBeNull();
        comp.Instance.CurrentDateRange!.Start.Should().Be(new DateTime(2023, 1, 2));
        comp.Instance.CurrentDateRange.End.Should().Be(new DateTime(2023, 1, 8));

        // Work Week View
        comp.SetParam(x => x.ShowWorkWeek, true);
        comp.SetParam(x => x.View, CalendarView.WorkWeek);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 14));
        comp.FindAll("button.mud-icon-button")[0].Click();
        day = comp.FindAll("div.mud-cal-grid.mud-cal-grid-header.mud-cal-work-week-header div")[1].TextContent;
        day.Substring(day.Length - 1, 1).Should().Be("1");
        comp.Instance.CurrentDateRange.Should().NotBeNull();
        comp.Instance.CurrentDateRange!.Start.Should().Be(new DateTime(2023, 1, 1));
        comp.Instance.CurrentDateRange.End.Should().Be(new DateTime(2023, 1, 5));

        // Day View
        comp.SetParam(x => x.View, CalendarView.Day);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 8));
        comp.FindAll("button.mud-icon-button")[0].Click();
        day = comp.FindAll("div.mud-cal-grid.mud-cal-grid-header.mud-cal-day-header div")[1].TextContent;
        day.Substring(day.Length - 1, 1).Should().Be("7");
        comp.Instance.CurrentDateRange.Should().NotBeNull();
        comp.Instance.CurrentDateRange!.Start.Should().Be(new DateTime(2023, 1, 7));
        comp.Instance.CurrentDateRange.End.Should().Be(new DateTime(2023, 1, 7));
    }

    [Test]
    public void CellClick()
    {
        var cut = Context.RenderComponent<CalendarCellClickTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        var textField = cut.FindComponents<MudTextField<string>>()[0];
        var timeField = cut.FindComponents<MudTextField<string>>()[1];
        
        // Month View
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 1));
        comp.Find("div.mud-drop-zone a").Click();
        textField.Instance.GetState(x => x.Text).Should().Be("26");
        timeField.Instance.GetState(x => x.Text).Should().Be("00:00");
        
        // Month View set with time
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 1, 9, 30, 0));
        comp.Find("div.mud-drop-zone a").Click();
        textField.Instance.GetState(x => x.Text).Should().Be("26");
        timeField.Instance.GetState(x => x.Text).Should().Be("00:00");

        // Work Week View
        comp.SetParam(x => x.View, CalendarView.WorkWeek);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 13));
        comp.Find("div.mud-cal-week-layer a").Click();
        textField.Instance.GetState(x => x.Text).Should().Be("13");
        
        // Work Week View set with time
        comp.SetParam(x => x.View, CalendarView.WorkWeek);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 13, 9, 30, 0));
        comp.FindAll("div.mud-cal-week-layer a")[55].Click();
        textField.Instance.GetState(x => x.Text).Should().Be("13");
        timeField.Instance.GetState(x => x.Text).Should().Be("09:10");

        // Week View
        comp.SetParam(x => x.View, CalendarView.Week);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 13));
        comp.Find("div.mud-cal-week-layer a").Click();
        textField.Instance.GetState(x => x.Text).Should().Be("9");
        
        // Week View set with time
        comp.SetParam(x => x.View, CalendarView.Week);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 13, 9, 30, 0));
        comp.FindAll("div.mud-cal-week-layer a")[55].Click();
        textField.Instance.GetState(x => x.Text).Should().Be("9");
        timeField.Instance.GetState(x => x.Text).Should().Be("09:10");
        
        // Day View
        comp.SetParam(x => x.View, CalendarView.Day);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 8));
        comp.FindAll("div.mud-cal-week-layer a")[55].Click();
        timeField.Instance.GetState(x => x.Text).Should().Be("09:10");
        
        // Day View set with time
        comp.SetParam(x => x.View, CalendarView.Day);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 8, 9, 30, 0));
        comp.FindAll("div.mud-cal-week-layer a")[55].Click();
        timeField.Instance.GetState(x => x.Text).Should().Be("09:10");   
    }

    [Test]
    public void CellContextMenuClick()
    {
        var cut = Context.RenderComponent<CalendarCellContextMenuClickTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        var textField = cut.FindComponents<MudTextField<string>>()[0];
        var timeField = cut.FindComponents<MudTextField<string>>()[1];
        
        // Month View
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 1));
        comp.Find("div.mud-drop-zone a").ContextMenu();
        textField.Instance.GetState(x => x.Text).Should().Be("26");
        timeField.Instance.GetState(x => x.Text).Should().Be("00:00");
        
        // Month View set with time
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 1, 9, 30, 0));
        comp.Find("div.mud-drop-zone a").ContextMenu();
        textField.Instance.GetState(x => x.Text).Should().Be("26");
        timeField.Instance.GetState(x => x.Text).Should().Be("00:00");

        // Work Week View
        comp.SetParam(x => x.View, CalendarView.WorkWeek);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 13));
        comp.Find("div.mud-cal-week-layer a").ContextMenu();
        textField.Instance.GetState(x => x.Text).Should().Be("13");
        
        // Work Week View set with time
        comp.SetParam(x => x.View, CalendarView.WorkWeek);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 13, 9, 30, 0));
        comp.FindAll("div.mud-cal-week-layer a")[55].ContextMenu();
        textField.Instance.GetState(x => x.Text).Should().Be("13");
        timeField.Instance.GetState(x => x.Text).Should().Be("09:10");

        // Week View
        comp.SetParam(x => x.View, CalendarView.Week);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 13));
        comp.Find("div.mud-cal-week-layer a").ContextMenu();
        textField.Instance.GetState(x => x.Text).Should().Be("9");
        
        // Week View set with time
        comp.SetParam(x => x.View, CalendarView.Week);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 13, 9, 30, 0));
        comp.FindAll("div.mud-cal-week-layer a")[55].ContextMenu();
        textField.Instance.GetState(x => x.Text).Should().Be("9");
        timeField.Instance.GetState(x => x.Text).Should().Be("09:10");
        
        // Day View
        comp.SetParam(x => x.View, CalendarView.Day);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 8));
        comp.FindAll("div.mud-cal-week-layer a")[55].ContextMenu();
        timeField.Instance.GetState(x => x.Text).Should().Be("09:10");
        
        // Day View set with time
        comp.SetParam(x => x.View, CalendarView.Day);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 8, 9, 30, 0));
        comp.FindAll("div.mud-cal-week-layer a")[55].ContextMenu();
        timeField.Instance.GetState(x => x.Text).Should().Be("09:10");   
    }
    
    [Test]
    public void DisabledDaysCellClick()
    {
        var cut = Context.RenderComponent<CalendarDisableDaysTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        var textField = cut.FindComponents<MudTextField<string>>()[0];
        var timeField = cut.FindComponents<MudTextField<string>>()[1];

        // Month View
        comp.SetParam(x => x.CurrentDay, new DateTime(2025, 5, 1));
        comp.FindAll("div.mud-drop-zone a")[4].Click();
        textField.Instance.GetState(x => x.Text).Should().Be("2");
        timeField.Instance.GetState(x => x.Text).Should().Be("00:00");
        comp.FindAll("div.mud-drop-zone")[4].ClassName.Should().NotContain("yellow-background");
        
        comp.FindAll("div.mud-drop-zone a")[5].Click(); // Weekend should be skipped
        textField.Instance.GetState(x => x.Text).Should().Be("5");
        timeField.Instance.GetState(x => x.Text).Should().Be("00:00");
        comp.FindAll("div.mud-drop-zone")[5].ClassName.Should().Contain("yellow-background");
        
        // Week View
        comp.SetParam(x => x.View, CalendarView.Week);
        comp.SetParam(x => x.CurrentDay, new DateTime(2025, 5, 1));
        comp.Find("div.mud-cal-week-layer a").Click();
        textField.Instance.GetState(x => x.Text).Should().Be("28");
        timeField.Instance.GetState(x => x.Text).Should().Be("08:00");
        comp.FindAll("div.mud-cal-week-cell")[1].ClassName.Should().Contain("yellow-background");
        comp.FindAll("div.mud-cal-week-cell")[128].ClassName.Should().NotContain("yellow-background");
        
        // Day View
        comp.SetParam(x => x.View, CalendarView.Day);
        comp.SetParam(x => x.CurrentDay, new DateTime(2025, 5, 1));
        comp.Find("div.mud-cal-week-layer a").Click();
        textField.Instance.GetState(x => x.Text).Should().Be("1");
        timeField.Instance.GetState(x => x.Text).Should().Be("08:00");
        comp.FindAll("div.mud-cal-week-cell")[1].ClassName.Should().Contain("yellow-background");
        comp.FindAll("div.mud-cal-week-cell")[32].ClassName.Should().NotContain("yellow-background");
    }

    [Test]
    public void ItemsClick()
    {
        var cut = Context.RenderComponent<CalendarItemClickTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        var textField = cut.FindComponent<MudTextField<string>>();
        
        // Month View
        comp.Find("div.mud-cal-cell-template").Click();
        textField.Instance.GetState(x => x.Text).Should().Be("Event_Month");
        
        // Week View
        comp.SetParam(x => x.View, CalendarView.Week);
        comp.Find("div.mud-cal-cell-template").Click();
        textField.Instance.GetState(x => x.Text).Should().Be("Event_Week");
        
        // Day View
        comp.SetParam(x => x.View, CalendarView.Day);
        comp.Find("div.mud-cal-cell-template").Click();
        textField.Instance.GetState(x => x.Text).Should().Be("Event_Day");
    }

    [Test]
    public void EnsureAllDays()
    {
        var cut = Context.RenderComponent<CalendarTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 1));
        comp.FindAll("div.mud-cal-month-cell-title").Count.Should().Be(42);
    }

    [Test]
    [SetCulture("en-US")]
    public void WeekStartSunday()
    {
        var cut = Context.RenderComponent<CalendarTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();

        // Check that the first day is a Sunday
        var firstDayDate = comp.FindAll("div.mud-drop-zone")[0].Attributes["identifier"]!.TextContent;
        DateTime.Parse(firstDayDate).DayOfWeek.Should().Be(DayOfWeek.Sunday);
    }
    
    [Test]
    public void FirstDayOfWeekTest()
    {
        var cut = Context.RenderComponent<CalendarFirstDayOfWeekTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();

        // Check that the first day is a Tuesday
        var firstDayDate = comp.FindAll("div.mud-drop-zone")[0].Attributes["identifier"]!.TextContent;
        DateTime.Parse(firstDayDate).DayOfWeek.Should().Be(DayOfWeek.Tuesday);
    }
    
    [Test]
    public void UpdateDatePicker()
    {
        var cut = Context.RenderComponent<CalendarTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();

        comp.SetParam(x => x.View, CalendarView.Week);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 2, 1));
        comp.FindAll("div.mud-cal-toolbar button")[0].Click();

        comp.Find("div.mud-picker button").Click();
        cut.Find("button.mud-button-month").TextContent.Should()
            .Be("January 2023");
    }

    [Test]
    public void EventOrder()
    {
        var cut = Context.RenderComponent<CalendarSameDayEventsTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        
        comp.Find("div.mud-cal-drop-item div.mud-cal-cell-template").TextContent.Should().Be("Event 1");
        
        cut.Find("button.add-item").Click();
        comp.FindAll("div.mud-cal-drop-item div.mud-cal-cell-template")[2].TextContent.Should().Be("Event 2.5");
    }

    [Test]
    [SetUICulture("de-DE")]
    public void ViewNameLocalization()
    {
        var cut = Context.RenderComponent<CalendarTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();

        comp.Find("div.mud-toggle-group button.mud-button-root span.mud-button-label").TextContent.Should()
            .Be("Monat");

        comp.FindAll("div.mud-cal-toolbar > div > button")[2].TextContent.Should().Be("Heute");
    }

    [Test]
    public void CellsClickable()
    {
        var cut = Context.RenderComponent<CalendarCellClickTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        
        // Month View
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 1));
        comp.Find("div.mud-cal-month-cell.mud-cal-month-link").Should().NotBeNull();

        // Week View
        comp.SetParam(x => x.View, CalendarView.Week);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 13));
        comp.Find("div.mud-cal-week-layer a").Should().NotBeNull();

        // Day View
        comp.SetParam(x => x.View, CalendarView.Day);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 8));
        comp.Find("div.mud-cal-week-layer a").Should().NotBeNull();
    }

    [Test]
    public void CellsNotClickable()
    {
        var cut = Context.RenderComponent<CalendarTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        
        // Month View
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 1));
        comp.FindAll("div.mud-cal-month-cell > div.mud-cal-month-link").Count.Should().Be(0);

        // Week View
        comp.SetParam(x => x.View, CalendarView.Week);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 13));
        comp.FindAll("div.mud-cal-week-layer a").Count.Should().Be(0);

        // Day View
        comp.SetParam(x => x.View, CalendarView.Day);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 8));
        comp.FindAll("div.mud-cal-week-layer a").Count.Should().Be(0);
    }

    [Test]
    public void OverlappingEvents()
    {
        var cut = Context.RenderComponent<CalendarOverlappingEventsTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        
        comp.SetParam(x => x.View, CalendarView.Day);
        var event1 = comp.FindAll("div.mud-cal-week-cell-holder > div.mud-cal-drop-item")[0];
        event1.Attributes["style"].Should().NotBeNull();
        event1.Attributes["style"]?.Value.Should().Contain("left:0");
        event1.Attributes["style"]?.Value.Should().Contain("width:33");
        
        var event2 = comp.FindAll("div.mud-cal-week-cell-holder > div.mud-cal-drop-item")[1];
        event2.Attributes["style"].Should().NotBeNull();
        event2.Attributes["style"]?.Value.Should().Contain("left:33");
        event2.Attributes["style"]?.Value.Should().Contain("width:33");
        
        // Make sure no error thrown on the event ending at midnight when go to the next day
        comp.FindAll("button.mud-icon-button")[1].Click();
        
        var event16 = comp.FindAll("div.mud-cal-week-cell-holder > div.mud-cal-drop-item")[0];
        event16.Attributes["style"].Should().NotBeNull();
        event16.Attributes["style"]?.Value.Should().Contain("left:0");
        event16.Attributes["style"]?.Value.Should().Contain("width:100");
        
        var event17 = comp.FindAll("div.mud-cal-week-cell-holder > div.mud-cal-drop-item")[1];
        event17.Attributes["style"].Should().NotBeNull();
        event17.Attributes["style"]?.Value.Should().Contain("left:0");
        event17.Attributes["style"]?.Value.Should().Contain("width:100");
    }

    [Test]
    public void MultiDayWorkWeekView()
    {
        var cut = Context.RenderComponent<CalendarMultiDayEventTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        comp.SetParam(x => x.ShowWorkWeek, true);
        comp.SetParam(x => x.ShowWeek, false);

        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 2, 1));
        comp.SetParam(x => x.View, CalendarView.WorkWeek);
        comp.FindAll("div.mud-cal-week-layer div:nth-child(4) div.mud-cal-cell-template").Count.Should().Be(1);
        comp.FindAll("div.mud-cal-week-layer div:nth-child(5) div.mud-cal-cell-template").Count.Should().Be(1);

        comp.SetParam(x => x.EnableDragItems, true);
        comp.FindAll("div.mud-cal-week-layer div:nth-child(4) div.mud-cal-cell-template").Count.Should().Be(1);
        comp.FindAll("div.mud-cal-week-layer div:nth-child(5) div.mud-cal-cell-template").Count.Should().Be(1);
    }

    [Test]
    public void MultiDayWeekView()
    {
        var cut = Context.RenderComponent<CalendarMultiDayEventTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 2, 1));
        comp.SetParam(x => x.View, CalendarView.Week);
        comp.FindAll("div.mud-cal-week-layer div:nth-child(4) div.mud-cal-cell-template").Count.Should().Be(1);
        comp.FindAll("div.mud-cal-week-layer div:nth-child(5) div.mud-cal-cell-template").Count.Should().Be(1);
        
        comp.SetParam(x => x.EnableDragItems, true);
        comp.FindAll("div.mud-cal-week-layer div:nth-child(4) div.mud-cal-cell-template").Count.Should().Be(1);
        comp.FindAll("div.mud-cal-week-layer div:nth-child(5) div.mud-cal-cell-template").Count.Should().Be(1);
    }
    
    [Test]
    public void MultiDayDayView()
    {
        var cut = Context.RenderComponent<CalendarMultiDayEventTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        
        comp.SetParam(x => x.View, CalendarView.Day);
        
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 2, 1));
        comp.FindAll("div.mud-cal-week-cell-holder div.mud-drop-item").Count.Should().Be(1);
        
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 2, 2));
        comp.FindAll("div.mud-cal-week-cell-holder div.mud-drop-item").Count.Should().Be(0);
        comp.FindAll("div.mud-cal-week-cell-holder div.mud-cal-cell-template").Count.Should().Be(1);
        
        comp.SetParam(x => x.EnableDragItems, true);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 2, 1));
        comp.FindAll("div.mud-cal-week-cell-holder div.mud-drop-item").Count.Should().Be(1);
        
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 2, 2));
        comp.FindAll("div.mud-cal-week-cell-holder div.mud-drop-item").Count.Should().Be(0);
        comp.FindAll("div.mud-cal-week-cell-holder div.mud-cal-cell-template").Count.Should().Be(1);
    }

    [Test]
    public void MultiDayMonthView()
    {
        var cut = Context.RenderComponent<CalendarMultiDayMonthTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        
        comp.SetParam(x => x.CurrentDay, new DateTime(2024, 11, 1));
        // Should be event 3
        comp.FindAll("div.mud-cal-drop-item")[0].Attributes["style"]!.TextContent.Contains("top:36px").Should().BeTrue();
        comp.FindAll("div.mud-cal-drop-item")[0].Attributes["style"]!.TextContent.Contains("inset-inline-start:14.2").Should().BeTrue();
        comp.FindAll("div.mud-cal-drop-item")[0].Attributes["style"]!.TextContent.Contains("width:28.5").Should().BeTrue();
        
        // Should be event 7
        comp.FindAll("div.mud-cal-drop-item")[8].Attributes["style"]!.TextContent.Contains("inset-inline-start:0").Should().BeTrue();
        comp.FindAll("div.mud-cal-drop-item")[8].Attributes["style"]!.TextContent.Contains("width:28.5").Should().BeTrue();
    }

    [Test]
    public void DateChangedEvent()
    {
        var cut = Context.RenderComponent<CalendarEventsTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();

        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 1));
        
        var table = cut.Find("tbody.daterange-events-table");
        table.Children.Length.Should().Be(2);
        
        comp.FindAll("button.mud-icon-button")[1].Click();
        table.Children.Length.Should().Be(3);
    }

    [Test]
    public void CurrentDayChangedEvent()
    {
        var cut = Context.RenderComponent<CalendarEventsTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();

        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 1));
        
        var table = cut.Find("tbody.currentday-events-table");
        table.Children.Length.Should().Be(0);
        
        comp.FindAll("button.mud-icon-button")[1].Click();
        table.Children.Length.Should().Be(1);

        var picker = cut.FindComponent<MudDatePicker>();
        picker.SetParam(p => p.Date!, new DateTime(2023, 1, 2));
        table.Children.Length.Should().Be(2);
    }

    [Test]
    public void ClockType()
    {
        var cut = Context.RenderComponent<CalendarTimeIntervalTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        
        // Check the 24-hour clock
        comp.FindAll("div.mud-cal-time-cell")[18].TextContent.Trim().Should().Be("18:00");

        // Check the 12-hour clock
        comp.SetParam(x => x.Use24HourClock, false);
        comp.FindAll("div.mud-cal-time-cell")[18].TextContent.Trim().ToLower().Should().Be("6 pm");
    }

    [Test]
    public void CurrentDayTest()
    {
        var cut = Context.RenderComponent<CalendarCurrentDayTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        
        // Check that the current month is Feb 2024
        comp.FindAll(".mud-drop-zone")[0].Attributes["identifier"]!.TextContent.Should().Be("29/01/2024");
    }

    [Test]
    public void MinMaxDayTest()
    {
        var cut = Context.RenderComponent<CalendarMinMaxDateTest>();
        var cal = cut.FindComponent<MudCalendar<CalendarItem>>();
        var picker = cut.FindComponent<MudDatePicker>();

        // Check that the date picker's min date is 1/1/2024
        picker.Instance.MinDate.Should().Be(new DateTime(2024, 1, 1));

        // Check that the date picker's current date is 1/1/2025
        picker.Instance.Date.Should().Be(new DateTime(2025, 1, 1));

        // Check that the date picker's max date is 12/31/2026
        picker.Instance.MaxDate.Should().Be(new DateTime(2026, 12, 31));

        // Click previous button 12 times to reach the min date
        for (int i = 0; i < 12; i++)
        {
            // Re-query the button each iteration to get fresh reference
            var prevButton = cal.FindAll("button.mud-icon-button")[0];
            prevButton.Attributes["aria-label"]?.Value.Should().Be("Previous Month");

            prevButton.Click();

            // Re-query again after click to check the updated state
            prevButton = cal.FindAll("button.mud-icon-button")[0];

            if (i < 11)
                prevButton.HasAttribute("disabled").Should().BeFalse();
            else
                prevButton.HasAttribute("disabled").Should().BeTrue();
        }

        // Reset to the middle date
        cal.SetParam(c => c.CurrentDay, new DateTime(2025, 1, 1));

        // Click next button 23 times to reach the max date
        for (int i = 0; i < 23; i++)
        {
            // Re-query the button each iteration to get fresh reference
            var nextButton = cal.FindAll("button.mud-icon-button")[1];
            nextButton.Attributes["aria-label"]?.Value.Should().Be("Next Month");

            nextButton.Click();

            // Re-query again after click to check the updated state
            nextButton = cal.FindAll("button.mud-icon-button")[1];

            if (i < 22)
                nextButton.HasAttribute("disabled").Should().BeFalse();
            else
                nextButton.HasAttribute("disabled").Should().BeTrue();
        }
    }

    [Test]
    public void DayItemMinHeightTest()
    {
        var cut = Context.RenderComponent<CalendarMinItemHeightTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        
        comp.SetParam(x => x.View, CalendarView.Day);
        var event1 = comp.FindAll("div.mud-cal-week-cell-holder > div.mud-cal-drop-item")[0];
        event1.Attributes["style"].Should().NotBeNull();
        event1.Attributes["style"]?.Value.Should().Contain("height:90");
        
        var event3 = comp.FindAll("div.mud-cal-week-cell-holder > div.mud-cal-drop-item")[2];
        event3.Attributes["style"].Should().NotBeNull();
        event3.Attributes["style"]?.Value.Should().Contain("width:50");
        
        var event4 = comp.FindAll("div.mud-cal-week-cell-holder > div.mud-cal-drop-item")[3];
        event4.Attributes["style"].Should().NotBeNull();
        event4.Attributes["style"]?.Value.Should().Contain("width:50");
        
        var event5 = comp.FindAll("div.mud-cal-week-cell-holder > div.mud-cal-drop-item")[4];
        event5.Attributes["style"].Should().NotBeNull();
        event5.Attributes["style"]?.Value.Should().Contain("height:50");
        
        var event6 = comp.FindAll("div.mud-cal-week-cell-holder > div.mud-cal-drop-item")[5];
        event6.Attributes["style"].Should().NotBeNull();
        event6.Attributes["style"]?.Value.Should().Contain("height:50");
    }

    [TestCase(CalendarView.Month)]
    [TestCase(CalendarView.Week)]
    [TestCase(CalendarView.Day)]
    public void ItemContextMenuClickedTest(CalendarView calendarView)
    {
        var cut = Context.RenderComponent<CalendarItemContextMenuClickTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        var itemTextField = cut.FindComponents<MudTextField<string>>()[1];

        // Set the view
        comp.SetParam(x => x.View, calendarView);

        // Before context menu click, the item text field should be empty
        itemTextField.Instance.GetState(x => x.Text).Should().BeNull();
        comp.Find("div.mud-cal-cell-template").ContextMenu();

        // After context menu click, the item text field should
        // contain the first event's Text value
        itemTextField.Instance.GetState(x => x.Text).Should().Be("Event 1");
    }
    
    [Test]
    public void VisibleTimes()
    {
        var cut = Context.RenderComponent<CalendarVisibleTimeTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        
        // Check the first time that is shown
        comp.FindAll("div.mud-cal-time-cell")[0].TextContent.Trim().Should().Be("07:00");
    }

    [Test]
    public void DragEnabledTest()
    {
        var cut = Context.RenderComponent<CalendarTest>();
        var comp = cut.FindComponent<MudCalendar<CalendarItem>>();
        
        // Month View - Drag should not be enabled
        comp.FindAll("div.mud-cal-drop-item .mud-drop-item[draggable=false]").Count.Should().BeGreaterThanOrEqualTo(1);
        
        // Day View - Drag should not be enabled
        comp.SetParam(x => x.View, CalendarView.Day);
        comp.FindAll("div.mud-cal-drop-item .mud-drop-item[draggable=false]").Count.Should().Be(1);
        
        // Enable dragging
        comp.SetParam(x => x.EnableDragItems, true);
        
        // Month View - Drag should be enabled
        comp.SetParam(x => x.View, CalendarView.Month);
        comp.FindAll("div.mud-cal-drop-item .mud-drop-item[draggable=true]").Count.Should().BeGreaterThanOrEqualTo(1);
        
        // Day View - Drag should be enabled
        comp.SetParam(x => x.View, CalendarView.Day);
        comp.FindAll("div.mud-cal-drop-item .mud-drop-item[draggable=true]").Count.Should().Be(1);
    }
}
