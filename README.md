# Kinesis

Kinesis is small & simple UI library for building terminal apps! It lets you build complex terminal interfaces using a cozy OOP shell that gets flattened into a data-oriented beast under the hood. All this, while (try) keeping a stable footprint—because your RAM has better things to do.

# Example: The "Not-so-scary" Counter

If you ever touched some UI library (Flutter, React) this will be familiar to you. If you haven't, don't worry I'll (try to) show you! So let's see "the beast"!

```csharp
public class App: Island {
    private int m_counter = 0;

    protected override Entity? Build() {
        return new OnUpdate<RenderMessage>(this) {
            Child = new ListOf {
                Children = new List<Entity> {
                    new UIText {
                        Name = "counter",
                        Text = "Nothing happened! :/"
                    }
                }
            },
            /* Not required using the 'msg' value. But can be useful! */
            On = (msg, visitor) => {
                /* 
                 * You can decide track each entity or just want read something from it.
                 * But if you not found the entity (name is not correct mostly), then return NULL! (Very scary)
                 */
                UIText? text = visitor.Visit<UIText>(name: "counter");
                text?.Text = $"Hey! Something is updated x {++m_counter}";
            }
        };
    }
}
```
See? It's that simple: you declare the UI elements you want to use in a tree structure. If you want some interactivity, just use OnUpdate<T> with an On callback! Easy-peasy!

# The Secret Sauce (How it works?)

"Okay, it's simple and cozy, but where is the 'beast' part?" — you might ask. The magic happens behind the scenes:
- __Island Flattening__: When you call Build(), Kinesis doesn't just keep a heavy object tree. It flattens everything into a lean, read-only list for the ECS core.
- __Priority Lane__: We don't like lag. Input information is strictly processed before rendering metadata. Always.
- __Zero-Allocation Goals__: The engine is a _gentleman_: not spamming the RAM with unpleasant bytes.

# Disclamer
Developed as a __Diploma Thesis__ project.
