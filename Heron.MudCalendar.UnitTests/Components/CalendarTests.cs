using System;
using FluentAssertions;
using Heron.MudCalendar.UnitTests.Viewer.TestComponents.Calendar;
using MudBlazor;

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
        Assert.AreEqual(3, buttons.Count);
        
        buttons[1].Click();
        cut.FindAll("div.mud-cal-week-view").Count.Should().Be(1);

        buttons[2].Click();
        cut.FindAll("div.mud-cal-week-view").Count.Should().Be(1);
    }

    [Test]
    public void RestrictViews()
    {
        var cut = Context.RenderComponent<CalendarTest>();
        var comp = cut.FindComponent<MudCalendar>();

        comp.SetParam(x => x.ShowDay, false);
        comp.SetParam(x => x.ShowWeek, false);

        comp.FindComponent<EnumSwitch<CalendarView>>().Find("div").ClassList.Should().Contain("d-none");
        cut.FindAll("div.mud-cal-month-view").Count.Should().Be(1);
    }

    [Test]
    public void EnsureViewIsChangedWhenCurrentViewNotAllowed()
    {
        var cut = Context.RenderComponent<CalendarTest>();
        var comp = cut.FindComponent<MudCalendar>();

        comp.SetParam(x => x.ShowMonth, false);

        cut.FindAll("div.mud-cal-month-view").Count.Should().Be(0);
        cut.FindAll("div.mud-cal-week-view").Count.Should().Be(1);
    }

    [Test]
    public void NextButton()
    {
        var cut = Context.RenderComponent<CalendarTest>();
        var comp = cut.FindComponent<MudCalendar>();
        
        // Month View
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 1));
        comp.FindAll("button.mud-icon-button")[1].Click();
        comp.FindAll("div.mud-cal-month-cell-title")[0].TextContent.Should().Be("30");
        
        // Week View
        comp.SetParam(x => x.View, CalendarView.Week);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 1));
        comp.FindAll("button.mud-icon-button")[1].Click();
        var day = comp.FindAll("div.mud-cal-week-view th")[1].TextContent;
        day.Substring(day.Length - 1, 1).Should().Be("2");
        
        // Day View
        comp.SetParam(x => x.View, CalendarView.Day);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 4));
        comp.FindAll("button.mud-icon-button")[1].Click();
        day = comp.FindAll("div.mud-cal-week-view th")[1].TextContent;
        day.Substring(day.Length - 1, 1).Should().Be("5");
    }

    [Test]
    public void PrevButton()
    {
        var cut = Context.RenderComponent<CalendarTest>();
        var comp = cut.FindComponent<MudCalendar>();
        
        // Month View
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 1));
        comp.FindAll("button.mud-icon-button")[0].Click();
        comp.Find("div.mud-cal-month-cell-title").TextContent.Should().Be("28");
        
        // Week View
        comp.SetParam(x => x.View, CalendarView.Week);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 13));
        comp.FindAll("button.mud-icon-button")[0].Click();
        var day = comp.FindAll("div.mud-cal-week-view th")[1].TextContent;
        day.Substring(day.Length - 1, 1).Should().Be("2");
        
        // Day View
        comp.SetParam(x => x.View, CalendarView.Day);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 8));
        comp.FindAll("button.mud-icon-button")[0].Click();
        day = comp.FindAll("div.mud-cal-week-view th")[1].TextContent;
        day.Substring(day.Length - 1, 1).Should().Be("7");
    }

    [Test]
    public void CellClick()
    {
        var cut = Context.RenderComponent<CalendarCellClickTest>();
        var comp = cut.FindComponent<MudCalendar>();
        var textField = cut.FindComponent<MudTextField<string>>();
        
        // Month View
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 1));
        comp.Find("div.mud-cal-month-cell.mud-cal-month-link").Click();
        textField.Instance.Text.Should().Be("26");
        
        // Week View
        comp.SetParam(x => x.View, CalendarView.Week);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 13));
        comp.Find("div.mud-cal-week-layer a").Click();
        textField.Instance.Text.Should().Be("9");
        
        // Day View
        comp.SetParam(x => x.View, CalendarView.Day);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 8));
        comp.Find("div.mud-cal-week-layer a").Click();
        textField.Instance.Text.Should().Be("8");
    }
    
    [Test]
    public void ItemsClick()
    {
        var cut = Context.RenderComponent<CalendarItemClickTest>();
        var comp = cut.FindComponent<MudCalendar>();
        var textField = cut.FindComponent<MudTextField<string>>();
        
        // Month View
        comp.Find("div.mud-cal-cell-template").Click();
        textField.Instance.Text.Should().Be("Event_Month");
        
        // Week View
        comp.SetParam(x => x.View, CalendarView.Week);
        comp.Find("div.mud-cal-cell-template").Click();
        textField.Instance.Text.Should().Be("Event_Week");
        
        // Day View
        comp.SetParam(x => x.View, CalendarView.Day);
        comp.Find("div.mud-cal-cell-template").Click();
        textField.Instance.Text.Should().Be("Event_Day");
    }

    [Test]
    public void EnsureAllDays()
    {
        var cut = Context.RenderComponent<CalendarTest>();
        var comp = cut.FindComponent<MudCalendar>();
        
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 1, 1));
        comp.FindAll("div.mud-cal-month-cell-title").Count.Should().Be(42);
    }

    [Test]
    [SetCulture("en-US")]
    public void WeekStartSunday()
    {
        var cut = Context.RenderComponent<CalendarTest>();
        var comp = cut.FindComponent<MudCalendar>();

        // Gets the highlighted day
        var today = comp.Find(
            "div.mud-cal-month-cell[style='border:1px solid var(--mud-palette-primary);'] div.mud-cal-month-cell-title");
        today.TextContent.Should().Be(DateTime.Today.Day.ToString());
    }

    [Test]
    public void UpdateDatePicker()
    {
        var cut = Context.RenderComponent<CalendarTest>();
        var comp = cut.FindComponent<MudCalendar>();
        
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
        var comp = cut.FindComponent<MudCalendar>();
        
        comp.Find("div.mud-cal-month-dropzone div.mud-cal-cell-template").TextContent.Should().Be("Event 1");
        
        cut.Find("button.add-item").Click();
        comp.FindAll("div.mud-cal-month-dropzone div.mud-cal-cell-template")[2].TextContent.Should().Be("Event 2.5");
    }

    [Test]
    [SetUICulture("de-DE")]
    public void ViewNameLocalization()
    {
        var cut = Context.RenderComponent<CalendarTest>();
        var comp = cut.FindComponent<MudCalendar>();

        comp.Find("div.mud-button-group-root button.mud-button-root span.mud-button-label").TextContent.Should()
            .Be("Monat");

        comp.FindAll("div.mud-cal-toolbar > div > button")[2].TextContent.Should().Be("Heute");
    }

    [Test]
    public void CellsClickable()
    {
        var cut = Context.RenderComponent<CalendarCellClickTest>();
        var comp = cut.FindComponent<MudCalendar>();
        
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
        var comp = cut.FindComponent<MudCalendar>();
        
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
        var comp = cut.FindComponent<MudCalendar>();
        
        comp.SetParam(x => x.View, CalendarView.Day);
        var event2 = comp.FindAll("td.mud-cal-week-cell-holder > div.mud-cal-week-drop-item")[1];
        event2.Attributes["style"].Should().NotBeNull();
        event2.Attributes["style"]?.Value.Should().Contain("left:33");
        event2.Attributes["style"]?.Value.Should().Contain("width:33");
    }

    [Test]
    public void MultiDayMonthView()
    {
        var cut = Context.RenderComponent<CalendarMultiDayEventTest>();
        var comp = cut.FindComponent<MudCalendar>();
        
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 2, 1));
        comp.FindAll("div.mud-cal-month-cell > div.mud-drop-zone > div.mud-cal-month-cell-title")[2].TextContent.Should().Be("1");
        comp.FindAll("div.mud-cal-month-cell > div.mud-drop-zone")[2].Children.Length.Should().Be(2);
        comp.FindAll("div.mud-cal-month-cell > div.mud-drop-zone")[3].Children.Length.Should().Be(2);
        
        comp.SetParam(x => x.EnableDragItems, true);
        comp.FindAll("div.mud-cal-month-cell > div.mud-drop-zone > div.mud-cal-month-cell-title")[2].TextContent.Should().Be("1");
        comp.FindAll("div.mud-cal-month-cell > div.mud-drop-zone")[2].Children.Length.Should().Be(2);
        comp.FindAll("div.mud-cal-month-cell > div.mud-drop-zone")[3].Children.Length.Should().Be(2);
    }

    [Test]
    public void MultiDayWeekView()
    {
        var cut = Context.RenderComponent<CalendarMultiDayEventTest>();
        var comp = cut.FindComponent<MudCalendar>();
        
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 2, 1));
        comp.SetParam(x => x.View, CalendarView.Week);
        comp.FindAll("div.mud-cal-week-layer tr td:nth-child(4) div.mud-cal-cell-template").Count.Should().Be(1);
        comp.FindAll("div.mud-cal-week-layer tr td:nth-child(5) div.mud-cal-cell-template").Count.Should().Be(1);
        
        comp.SetParam(x => x.EnableDragItems, true);
        comp.FindAll("div.mud-cal-week-layer tr td:nth-child(4) div.mud-cal-cell-template").Count.Should().Be(1);
        comp.FindAll("div.mud-cal-week-layer tr td:nth-child(5) div.mud-cal-cell-template").Count.Should().Be(1);
    }
    
    [Test]
    public void MultiDayDayView()
    {
        var cut = Context.RenderComponent<CalendarMultiDayEventTest>();
        var comp = cut.FindComponent<MudCalendar>();
        
        comp.SetParam(x => x.View, CalendarView.Day);
        
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 2, 1));
        comp.FindAll("td.mud-cal-week-cell-holder div.mud-drop-item").Count.Should().Be(1);
        
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 2, 2));
        comp.FindAll("td.mud-cal-week-cell-holder div.mud-drop-item").Count.Should().Be(0);
        comp.FindAll("td.mud-cal-week-cell-holder div.mud-cal-cell-template").Count.Should().Be(1);
        
        comp.SetParam(x => x.EnableDragItems, true);
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 2, 1));
        comp.FindAll("td.mud-cal-week-cell-holder div.mud-drop-item").Count.Should().Be(1);
        
        comp.SetParam(x => x.CurrentDay, new DateTime(2023, 2, 2));
        comp.FindAll("td.mud-cal-week-cell-holder div.mud-drop-item").Count.Should().Be(0);
        comp.FindAll("td.mud-cal-week-cell-holder div.mud-cal-cell-template").Count.Should().Be(1);
    }

    [Test]
    public void DateChangedEvent()
    {
        var cut = Context.RenderComponent<CalendarEventsTest>();
        var comp = cut.FindComponent<MudCalendar>();

        var table = cut.Find("tbody.events-table");
        table.Children.Length.Should().Be(1);
        
        comp.FindAll("button.mud-icon-button")[1].Click();
        table.Children.Length.Should().Be(2);
    }

    [Test]
    public void ClockType()
    {
        var cut = Context.RenderComponent<CalendarTimeIntervalTest>();
        var comp = cut.FindComponent<MudCalendar>();
        
        // Check 24 hour clock
        comp.FindAll("td.mud-cal-time-cell")[18].TextContent.Trim().Should().Be("18:00");

        // Check 12 hour clock
        comp.SetParam(x => x.Use24HourClock, false);
        comp.FindAll("td.mud-cal-time-cell")[18].TextContent.Trim().Should().Be("6 pm");
    }
}
