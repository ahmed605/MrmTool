using Windows.UI.Xaml;
using WinUIEditor;

namespace MrmTool.Scintilla
{
    internal unsafe static class EditorExtensions
    {
        internal static void HandleSyntaxHighlightingApplied(this CodeEditorControl control, ElementTheme theme)
        {
            if (control.HighlightingLanguage is "css")
            {
                var editor = control.Editor;

                var lexer = Lexilla.CreateLexer("css");
                lexer->PropertySetUnsafe("fold"u8, "1"u8);
                lexer->PropertySetUnsafe("lexer.css.scss.language"u8, "1"u8);
                editor.SetILexer((ulong)lexer);

                editor.SetKeyWords(0, "color background background-color font font-size font-family font-weight " +
                                      "margin margin-top margin-right margin-bottom margin-left " +
                                      "padding padding-top padding-right padding-bottom padding-left " +
                                      "border border-radius width height display position top right bottom left " +
                                      "align-items justify-content flex grid gap");

                editor.SetKeyWords(1, "hover active focus visited disabled checked first-child last-child " +
                                      "nth-child nth-of-type not before after");

                if (theme is ElementTheme.Dark)
                {
                    editor.StyleSetFore((int)StylesCommon.Default, DarkPlusTheme.DarkPlusEditorForeground);
                    editor.StyleSetFore((int)StylesCommon.BraceLight, DarkPlusTheme.DarkPlusEditorForeground);

                    editor.StyleSetFore(Lexilla.SCE_CSS_DEFAULT, DarkPlusTheme.DarkPlusEditorForeground);
                    editor.StyleSetFore(Lexilla.SCE_CSS_TAG, DarkPlusTheme.Colors[(int)Scope.EntityNameTagCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_CLASS, DarkPlusTheme.Colors[(int)Scope.EntityOtherAttribute_NameClassCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_ID, DarkPlusTheme.Colors[(int)Scope.EntityOtherAttribute_NameIdCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_ATTRIBUTE, DarkPlusTheme.Colors[(int)Scope.EntityOtherAttribute_Name]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_PSEUDOCLASS, DarkPlusTheme.Colors[(int)Scope.EntityOtherAttribute_NamePseudo_ClassCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_PSEUDOELEMENT, DarkPlusTheme.Colors[(int)Scope.EntityOtherAttribute_NamePseudo_ElementCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_EXTENDED_PSEUDOCLASS, DarkPlusTheme.Colors[(int)Scope.EntityOtherAttribute_NamePseudo_ClassCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_EXTENDED_PSEUDOELEMENT, DarkPlusTheme.Colors[(int)Scope.EntityOtherAttribute_NamePseudo_ElementCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_IDENTIFIER, DarkPlusTheme.Colors[(int)Scope.VariableCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_IDENTIFIER2, DarkPlusTheme.Colors[(int)Scope.VariableCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_IDENTIFIER3, DarkPlusTheme.Colors[(int)Scope.VariableCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_EXTENDED_IDENTIFIER, DarkPlusTheme.Colors[(int)Scope.VariableCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_VALUE, DarkPlusTheme.Colors[(int)Scope.ConstantNumeric]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_DOUBLESTRING, DarkPlusTheme.Colors[(int)Scope.String]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_SINGLESTRING, DarkPlusTheme.Colors[(int)Scope.String]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_OPERATOR, DarkPlusTheme.Colors[(int)Scope.KeywordOperator]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_COMMENT, DarkPlusTheme.Colors[(int)Scope.Comment]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_DIRECTIVE, DarkPlusTheme.Colors[(int)Scope.MetaPreprocessor]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_GROUP_RULE, DarkPlusTheme.Colors[(int)Scope.MetaPreprocessor]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_IMPORTANT, DarkPlusTheme.Colors[(int)Scope.Keyword]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_VARIABLE, DarkPlusTheme.Colors[(int)Scope.VariableCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_UNKNOWN_PSEUDOCLASS, DarkPlusTheme.Colors[(int)Scope.EntityOtherAttribute_NamePseudo_ClassCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_UNKNOWN_IDENTIFIER, DarkPlusTheme.Colors[(int)Scope.VariableCss]);
                }
                else
                {
                    editor.StyleSetFore((int)StylesCommon.Default, LightPlusTheme.LightPlusEditorForeground);
                    editor.StyleSetFore((int)StylesCommon.BraceLight, LightPlusTheme.LightPlusEditorForeground);

                    editor.StyleSetFore(Lexilla.SCE_CSS_DEFAULT, LightPlusTheme.LightPlusEditorForeground);
                    editor.StyleSetFore(Lexilla.SCE_CSS_TAG, LightPlusTheme.Colors[(int)Scope.EntityNameTagCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_CLASS, LightPlusTheme.Colors[(int)Scope.EntityOtherAttribute_NameClassCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_ID, LightPlusTheme.Colors[(int)Scope.EntityOtherAttribute_NameIdCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_ATTRIBUTE, LightPlusTheme.Colors[(int)Scope.EntityOtherAttribute_Name]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_PSEUDOCLASS, LightPlusTheme.Colors[(int)Scope.EntityOtherAttribute_NamePseudo_ClassCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_PSEUDOELEMENT, LightPlusTheme.Colors[(int)Scope.EntityOtherAttribute_NamePseudo_ElementCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_EXTENDED_PSEUDOCLASS, LightPlusTheme.Colors[(int)Scope.EntityOtherAttribute_NamePseudo_ClassCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_EXTENDED_PSEUDOELEMENT, LightPlusTheme.Colors[(int)Scope.EntityOtherAttribute_NamePseudo_ElementCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_IDENTIFIER, LightPlusTheme.Colors[(int)Scope.VariableCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_IDENTIFIER2, LightPlusTheme.Colors[(int)Scope.VariableCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_IDENTIFIER3, LightPlusTheme.Colors[(int)Scope.VariableCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_EXTENDED_IDENTIFIER, LightPlusTheme.Colors[(int)Scope.VariableCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_VALUE, LightPlusTheme.Colors[(int)Scope.ConstantNumeric]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_DOUBLESTRING, LightPlusTheme.Colors[(int)Scope.String]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_SINGLESTRING, LightPlusTheme.Colors[(int)Scope.String]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_OPERATOR, LightPlusTheme.Colors[(int)Scope.KeywordOperator]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_COMMENT, LightPlusTheme.Colors[(int)Scope.Comment]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_DIRECTIVE, LightPlusTheme.Colors[(int)Scope.MetaPreprocessor]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_GROUP_RULE, LightPlusTheme.Colors[(int)Scope.MetaPreprocessor]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_IMPORTANT, LightPlusTheme.Colors[(int)Scope.Keyword]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_VARIABLE, LightPlusTheme.Colors[(int)Scope.VariableCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_UNKNOWN_PSEUDOCLASS, LightPlusTheme.Colors[(int)Scope.EntityOtherAttribute_NamePseudo_ClassCss]);
                    editor.StyleSetFore(Lexilla.SCE_CSS_UNKNOWN_IDENTIFIER, LightPlusTheme.Colors[(int)Scope.VariableCss]);
                }
            }
        }
    }
}
