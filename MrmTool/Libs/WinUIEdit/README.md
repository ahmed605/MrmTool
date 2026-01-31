### These binary blobs are compiled from https://github.com/ahmed605/WinUIEdit (be85c42a9dc23d5043b93f5e07695be0b47d85a0)

To build your own binaries,
1. Clone the repository and follow the **UWP** [build instructions](https://github.com/BreeceW/WinUIEdit#how-to-build-this-project) from the repository's README
2. Copy
   - `WinUIEditor.pri` and `WinUIEditor.winmd` files and `WinUIEditor` folder from `_buildUwp\x64\Release\WinUIEditor\bin\WinUIEditor` folder
   - `WinUIEditorCsWinRT.dll` from `_buildUwp\AnyCPU\Release\net8.0-windows10.0.22621.0\WinUIEditorCsWinRT\bin` folder
   - `WinUIEditorCsWinRT.xml` and `WinUIEditor.xml` from `WinUIEditorCsWinRT\nuget` folder to `MrmTool\Libs\WinUIEdit` folder in this repository
3. For each arch copy `WinUIEditor.dll` from `_buildUwp\[arch]\Release\WinUIEditor\bin\WinUIEditor` to `MrmTool\Libs\WinUIEdit\[arch]` folder in this repository.
