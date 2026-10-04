using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Tests.Helpers;

public static class Keys
{
    public static KeyboardEventArgs Enter => Create("Enter", "Enter");

    public static KeyboardEventArgs Space => Create(" ", "Space");

    public static KeyboardEventArgs Escape => Create("Escape", "Escape");

    public static KeyboardEventArgs Tab => Create("Tab", "Tab");

    public static KeyboardEventArgs ShiftTab => Create("Tab", "Tab", shift: true);

    public static KeyboardEventArgs ArrowUp => Create("ArrowUp", "ArrowUp");

    public static KeyboardEventArgs ArrowDown => Create("ArrowDown", "ArrowDown");

    public static KeyboardEventArgs ArrowLeft => Create("ArrowLeft", "ArrowLeft");

    public static KeyboardEventArgs ArrowRight => Create("ArrowRight", "ArrowRight");

    public static KeyboardEventArgs Home => Create("Home", "Home");

    public static KeyboardEventArgs End => Create("End", "End");

    public static KeyboardEventArgs Backspace => Create("Backspace", "Backspace");

    public static KeyboardEventArgs Character(char value) =>
        Create(value.ToString(), char.IsLetter(value) ? $"Key{char.ToUpperInvariant(value)}" : string.Empty);

    private static KeyboardEventArgs Create(string key, string code, bool shift = false) =>
        new() { Key = key, Code = code, ShiftKey = shift, Type = "keydown" };
}
