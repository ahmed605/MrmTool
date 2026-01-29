# MrmTool

**MrmTool** is a tool for viewing and editing **PRI** files, it allows you to create, modify, and remove **PRI** resources as well as previewing their contents.

**MrmTool** relies on [MrmLib](https://github.com/ahmed605/MrmLib) for processing and modifying **PRI** files.

It also includes an **XBF** (XAML Binary Format) decompiler, it currently uses a modified version of [XbfAnalyzer](https://github.com/chausner/XbfAnalyzer) (the modified version is based on an older commit and not latest), but it's [planned](https://github.com/ahmed605/MrmTool/blob/f48c57a23fb1c53ac82dcbd3e9b0418206740dca/MrmTool/PriPage.xaml.cs#L472-L475) to be replaced with a new decompiler/recompiler based on **WinUI 3**' **XBF** parser.

**MrmTool** supports the following **PRI** versions:
- Windows 8 (`mrm_pri0`)
- Windows 8.1 (`mrm_pri1`)
- Windows Phone 8.1 (`mrm_prif`)
- UWP (`mrm_pri2`)
- UWP RS4+ (`mrm_pri3`)
- Windows App SDK / WinUI 3 (`mrm_pri3`)
- UWP vNext (`mrm_vnxt`)

### Screenshot

<img width="1557" height="914" alt="MrmTool_0" src="https://github.com/user-attachments/assets/da515d3e-ff34-4173-8691-9f2ad9a3f429" />
