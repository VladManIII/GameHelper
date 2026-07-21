namespace HookLib.Interfaces;

public interface IMouseController
{
    void DownLeftKey();
    void UpLeftKey();
    void PressLeftKey();
    void PressLeftKey(int x, int y);

    void DownRightKey();
    void UpRightKey();
    void PressRightKey();
    void PressRightKey(int x, int y);

    void MoveTo(int x, int y, bool emulateReal = false);
}