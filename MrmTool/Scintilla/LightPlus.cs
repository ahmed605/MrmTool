// Ported from
// https://github.com/microsoft/vscode/blob/main/extensions/theme-defaults/themes/light_plus.json
// https://github.com/microsoft/vscode/blob/main/extensions/theme-defaults/themes/light_vs.json
// under the following license

/*
    MIT License

    Copyright (c) 2015 - present Microsoft Corporation

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

namespace MrmTool.Scintilla
{
    internal static class LightPlusTheme
    {
        internal const int LightPlusEditorForeground = unchecked((int)0xFF000000);

        internal static ReadOnlySpan<int> Colors
        {
            get =>
            [
                unchecked((int)0xFF000000), // MetaEmbedded
                unchecked((int)0xFF000000), // SourceGroovyEmbedded
                unchecked((int)0xFF000000), // String__MetaImageInlineMarkdown
                unchecked((int)0xFF000000), // VariableLegacyBuiltinPython
                unchecked((int)0xFF800000), // MetaDiffHeader
                unchecked((int)0xFF008000), // Comment
                unchecked((int)0xFFFF0000), // ConstantLanguage
                unchecked((int)0xFF588609), // ConstantNumeric
                unchecked((int)0xFFC17000), // VariableOtherEnummember
                unchecked((int)0xFF588609), // KeywordOperatorPlusExponent
                unchecked((int)0xFF588609), // KeywordOperatorMinusExponent
                unchecked((int)0xFF3F1F81), // ConstantRegexp
                unchecked((int)0xFF000080), // EntityNameTag
                unchecked((int)0xFF000080), // EntityNameSelector
                unchecked((int)0xFF0000E5), // EntityOtherAttribute_Name
                unchecked((int)0xFF000080), // EntityOtherAttribute_NameClassCss
                unchecked((int)0xFF000080), // EntityOtherAttribute_NameClassMixinCss
                unchecked((int)0xFF000080), // EntityOtherAttribute_NameIdCss
                unchecked((int)0xFF000080), // EntityOtherAttribute_NameParent_SelectorCss
                unchecked((int)0xFF000080), // EntityOtherAttribute_NamePseudo_ClassCss
                unchecked((int)0xFF000080), // EntityOtherAttribute_NamePseudo_ElementCss
                unchecked((int)0xFF000080), // SourceCssLess__EntityOtherAttribute_NameId
                unchecked((int)0xFF000080), // EntityOtherAttribute_NameScss
                unchecked((int)0xFF3131CD), // Invalid
                unchecked((int)0xFF800000), // MarkupBold
                unchecked((int)0xFF000080), // MarkupHeading
                unchecked((int)0xFF588609), // MarkupInserted
                unchecked((int)0xFF1515A3), // MarkupDeleted
                unchecked((int)0xFFA55104), // MarkupChanged
                unchecked((int)0xFFA55104), // PunctuationDefinitionQuoteBeginMarkdown
                unchecked((int)0xFFA55104), // PunctuationDefinitionListBeginMarkdown
                unchecked((int)0xFF000080), // MarkupInlineRaw
                unchecked((int)0xFF000080), // PunctuationDefinitionTag
                unchecked((int)0xFF808080), // MetaPreprocessor
                unchecked((int)0xFFFF0000), // EntityNameFunctionPreprocessor
                unchecked((int)0xFF1515A3), // MetaPreprocessorString
                unchecked((int)0xFF588609), // MetaPreprocessorNumeric
                unchecked((int)0xFFA55104), // MetaStructureDictionaryKeyPython
                unchecked((int)0xFFFF0000), // Storage
                unchecked((int)0xFFFF0000), // StorageType
                unchecked((int)0xFFFF0000), // StorageModifier
                unchecked((int)0xFFFF0000), // KeywordOperatorNoexcept
                unchecked((int)0xFF1515A3), // String
                unchecked((int)0xFF1515A3), // MetaEmbeddedAssembly
                unchecked((int)0xFFFF0000), // StringCommentBufferedBlockPug
                unchecked((int)0xFFFF0000), // StringQuotedPug
                unchecked((int)0xFFFF0000), // StringInterpolatedPug
                unchecked((int)0xFFFF0000), // StringUnquotedPlainInYaml
                unchecked((int)0xFFFF0000), // StringUnquotedPlainOutYaml
                unchecked((int)0xFFFF0000), // StringUnquotedBlockYaml
                unchecked((int)0xFFFF0000), // StringQuotedSingleYaml
                unchecked((int)0xFFFF0000), // StringQuotedDoubleXml
                unchecked((int)0xFFFF0000), // StringQuotedSingleXml
                unchecked((int)0xFFFF0000), // StringUnquotedCdataXml
                unchecked((int)0xFFFF0000), // StringQuotedDoubleHtml
                unchecked((int)0xFFFF0000), // StringQuotedSingleHtml
                unchecked((int)0xFFFF0000), // StringUnquotedHtml
                unchecked((int)0xFFFF0000), // StringQuotedSingleHandlebars
                unchecked((int)0xFFFF0000), // StringQuotedDoubleHandlebars
                unchecked((int)0xFF3F1F81), // StringRegexp
                unchecked((int)0xFFFF0000), // PunctuationDefinitionTemplate_ExpressionBegin
                unchecked((int)0xFFFF0000), // PunctuationDefinitionTemplate_ExpressionEnd
                unchecked((int)0xFFFF0000), // PunctuationSectionEmbedded
                unchecked((int)0xFF000000), // MetaTemplateExpression
                unchecked((int)0xFFA55104), // SupportConstantProperty_Value
                unchecked((int)0xFFA55104), // SupportConstantFont_Name
                unchecked((int)0xFFA55104), // SupportConstantMedia_Type
                unchecked((int)0xFFA55104), // SupportConstantMedia
                unchecked((int)0xFFA55104), // ConstantOtherColorRgb_Value
                unchecked((int)0xFFA55104), // ConstantOtherRgb_Value
                unchecked((int)0xFFA55104), // SupportConstantColor
                unchecked((int)0xFF0000E5), // SupportTypeVendoredProperty_Name
                unchecked((int)0xFF0000E5), // SupportTypeProperty_Name
                unchecked((int)0xFF0000E5), // VariableCss
                unchecked((int)0xFF0000E5), // VariableScss
                unchecked((int)0xFF0000E5), // VariableOtherLess
                unchecked((int)0xFF0000E5), // SourceCoffeeEmbedded
                unchecked((int)0xFFA55104), // SupportTypeProperty_NameJson
                unchecked((int)0xFFFF0000), // Keyword
                unchecked((int)0xFFDB00AF), // KeywordControl
                unchecked((int)0xFF000000), // KeywordOperator
                unchecked((int)0xFFFF0000), // KeywordOperatorNew
                unchecked((int)0xFFFF0000), // KeywordOperatorExpression
                unchecked((int)0xFFFF0000), // KeywordOperatorCast
                unchecked((int)0xFFFF0000), // KeywordOperatorSizeof
                unchecked((int)0xFFFF0000), // KeywordOperatorAlignof
                unchecked((int)0xFFFF0000), // KeywordOperatorTypeid
                unchecked((int)0xFFFF0000), // KeywordOperatorAlignas
                unchecked((int)0xFFFF0000), // KeywordOperatorInstanceof
                unchecked((int)0xFFFF0000), // KeywordOperatorLogicalPython
                unchecked((int)0xFFFF0000), // KeywordOperatorWordlike
                unchecked((int)0xFF588609), // KeywordOtherUnit
                unchecked((int)0xFF000080), // PunctuationSectionEmbeddedBeginPhp
                unchecked((int)0xFF000080), // PunctuationSectionEmbeddedEndPhp
                unchecked((int)0xFFA55104), // SupportFunctionGit_Rebase
                unchecked((int)0xFF588609), // ConstantShaGit_Rebase
                unchecked((int)0xFF000000), // StorageModifierImportJava
                unchecked((int)0xFF000000), // VariableLanguageWildcardJava
                unchecked((int)0xFF000000), // StorageModifierPackageJava
                unchecked((int)0xFFFF0000), // VariableLanguage
                unchecked((int)0xFF265E79), // EntityNameFunction
                unchecked((int)0xFF265E79), // SupportFunction
                unchecked((int)0xFF265E79), // SupportConstantHandlebars
                unchecked((int)0xFF265E79), // SourcePowershell__VariableOtherMember
                unchecked((int)0xFF265E79), // EntityNameOperatorCustom_Literal
                unchecked((int)0xFF997F26), // SupportClass
                unchecked((int)0xFF997F26), // SupportType
                unchecked((int)0xFF997F26), // EntityNameType
                unchecked((int)0xFF997F26), // EntityNameNamespace
                unchecked((int)0xFF997F26), // EntityOtherAttribute
                unchecked((int)0xFF997F26), // EntityNameScope_Resolution
                unchecked((int)0xFF997F26), // EntityNameClass
                unchecked((int)0xFF997F26), // StorageTypeNumericGo
                unchecked((int)0xFF997F26), // StorageTypeByteGo
                unchecked((int)0xFF997F26), // StorageTypeBooleanGo
                unchecked((int)0xFF997F26), // StorageTypeStringGo
                unchecked((int)0xFF997F26), // StorageTypeUintptrGo
                unchecked((int)0xFF997F26), // StorageTypeErrorGo
                unchecked((int)0xFF997F26), // StorageTypeRuneGo
                unchecked((int)0xFF997F26), // StorageTypeCs
                unchecked((int)0xFF997F26), // StorageTypeGenericCs
                unchecked((int)0xFF997F26), // StorageTypeModifierCs
                unchecked((int)0xFF997F26), // StorageTypeVariableCs
                unchecked((int)0xFF997F26), // StorageTypeAnnotationJava
                unchecked((int)0xFF997F26), // StorageTypeGenericJava
                unchecked((int)0xFF997F26), // StorageTypeJava
                unchecked((int)0xFF997F26), // StorageTypeObjectArrayJava
                unchecked((int)0xFF997F26), // StorageTypePrimitiveArrayJava
                unchecked((int)0xFF997F26), // StorageTypePrimitiveJava
                unchecked((int)0xFF997F26), // StorageTypeTokenJava
                unchecked((int)0xFF997F26), // StorageTypeGroovy
                unchecked((int)0xFF997F26), // StorageTypeAnnotationGroovy
                unchecked((int)0xFF997F26), // StorageTypeParametersGroovy
                unchecked((int)0xFF997F26), // StorageTypeGenericGroovy
                unchecked((int)0xFF997F26), // StorageTypeObjectArrayGroovy
                unchecked((int)0xFF997F26), // StorageTypePrimitiveArrayGroovy
                unchecked((int)0xFF997F26), // StorageTypePrimitiveGroovy
                unchecked((int)0xFF997F26), // MetaTypeCastExpr
                unchecked((int)0xFF997F26), // MetaTypeNewExpr
                unchecked((int)0xFF997F26), // SupportConstantMath
                unchecked((int)0xFF997F26), // SupportConstantDom
                unchecked((int)0xFF997F26), // SupportConstantJson
                unchecked((int)0xFF997F26), // EntityOtherInherited_Class
                unchecked((int)0xFFDB00AF), // SourceCpp__KeywordOperatorNew
                unchecked((int)0xFFDB00AF), // SourceCpp__KeywordOperatorDelete
                unchecked((int)0xFFDB00AF), // KeywordOtherUsing
                unchecked((int)0xFFDB00AF), // KeywordOtherDirectiveUsing
                unchecked((int)0xFFDB00AF), // KeywordOtherOperator
                unchecked((int)0xFFDB00AF), // EntityNameOperator
                unchecked((int)0xFF801000), // Variable
                unchecked((int)0xFF801000), // MetaDefinitionVariableName
                unchecked((int)0xFF801000), // SupportVariable
                unchecked((int)0xFF801000), // EntityNameVariable
                unchecked((int)0xFF801000), // ConstantOtherPlaceholder
                unchecked((int)0xFFC17000), // VariableOtherConstant
                unchecked((int)0xFF801000), // MetaObject_LiteralKey
                unchecked((int)0xFF6969D1), // PunctuationDefinitionGroupRegexp
                unchecked((int)0xFF6969D1), // PunctuationDefinitionGroupAssertionRegexp
                unchecked((int)0xFF6969D1), // PunctuationDefinitionCharacter_ClassRegexp
                unchecked((int)0xFF6969D1), // PunctuationCharacterSetBeginRegexp
                unchecked((int)0xFF6969D1), // PunctuationCharacterSetEndRegexp
                unchecked((int)0xFF6969D1), // KeywordOperatorNegationRegexp
                unchecked((int)0xFF6969D1), // SupportOtherParenthesisRegexp
                unchecked((int)0xFF3F1F81), // ConstantCharacterCharacter_ClassRegexp
                unchecked((int)0xFF3F1F81), // ConstantOtherCharacter_ClassSetRegexp
                unchecked((int)0xFF3F1F81), // ConstantOtherCharacter_ClassRegexp
                unchecked((int)0xFF3F1F81), // ConstantCharacterSetRegexp
                unchecked((int)0xFF000000), // KeywordOperatorQuantifierRegexp
                unchecked((int)0xFF0000EE), // KeywordOperatorOrRegexp
                unchecked((int)0xFF0000EE), // KeywordControlAnchorRegexp
                unchecked((int)0xFFFF0000), // ConstantCharacter
                unchecked((int)0xFFFF0000), // ConstantOtherOption
                unchecked((int)0xFF0000EE), // ConstantCharacterEscape
                unchecked((int)0xFF000000), // EntityNameLabel
                unchecked((int)0xFF000000), // Header
                unchecked((int)0xFF000080), // EntityNameTagCss
                unchecked((int)0xFF000000), // StringTag
                unchecked((int)0xFF000000), // StringValue
                unchecked((int)0xFF000000), // KeywordOperatorDelete
            ];
        }

#if ENABLE_SLOW_THEME_COLOR_GETTERS
        internal static int LightPlus(Scope scope)
        {
            switch (scope)
            {
                case Scope.MetaEmbedded: return unchecked((int)0xFF000000);
                case Scope.SourceGroovyEmbedded: return unchecked((int)0xFF000000);
                case Scope.String__MetaImageInlineMarkdown: return unchecked((int)0xFF000000);
                case Scope.VariableLegacyBuiltinPython: return unchecked((int)0xFF000000);
                case Scope.MetaDiffHeader: return unchecked((int)0xFF800000);
                case Scope.Comment: return unchecked((int)0xFF008000);
                case Scope.ConstantLanguage: return unchecked((int)0xFFFF0000);
                case Scope.ConstantNumeric: return unchecked((int)0xFF588609);
                case Scope.VariableOtherEnummember: return unchecked((int)0xFFC17000);
                case Scope.KeywordOperatorPlusExponent: return unchecked((int)0xFF588609);
                case Scope.KeywordOperatorMinusExponent: return unchecked((int)0xFF588609);
                case Scope.ConstantRegexp: return unchecked((int)0xFF3F1F81);
                case Scope.EntityNameTag: return unchecked((int)0xFF000080);
                case Scope.EntityNameSelector: return unchecked((int)0xFF000080);
                case Scope.EntityOtherAttribute_Name: return unchecked((int)0xFF0000E5);
                case Scope.EntityOtherAttribute_NameClassCss: return unchecked((int)0xFF000080);
                case Scope.EntityOtherAttribute_NameClassMixinCss: return unchecked((int)0xFF000080);
                case Scope.EntityOtherAttribute_NameIdCss: return unchecked((int)0xFF000080);
                case Scope.EntityOtherAttribute_NameParent_SelectorCss: return unchecked((int)0xFF000080);
                case Scope.EntityOtherAttribute_NamePseudo_ClassCss: return unchecked((int)0xFF000080);
                case Scope.EntityOtherAttribute_NamePseudo_ElementCss: return unchecked((int)0xFF000080);
                case Scope.SourceCssLess__EntityOtherAttribute_NameId: return unchecked((int)0xFF000080);
                case Scope.EntityOtherAttribute_NameScss: return unchecked((int)0xFF000080);
                case Scope.Invalid: return unchecked((int)0xFF3131CD);
                case Scope.MarkupBold: return unchecked((int)0xFF800000);
                case Scope.MarkupHeading: return unchecked((int)0xFF000080);
                case Scope.MarkupInserted: return unchecked((int)0xFF588609);
                case Scope.MarkupDeleted: return unchecked((int)0xFF1515A3);
                case Scope.MarkupChanged: return unchecked((int)0xFFA55104);
                case Scope.PunctuationDefinitionQuoteBeginMarkdown: return unchecked((int)0xFFA55104);
                case Scope.PunctuationDefinitionListBeginMarkdown: return unchecked((int)0xFFA55104);
                case Scope.MarkupInlineRaw: return unchecked((int)0xFF000080);
                case Scope.PunctuationDefinitionTag: return unchecked((int)0xFF000080);
                case Scope.MetaPreprocessor: return unchecked((int)0xFFFF0000);
                case Scope.EntityNameFunctionPreprocessor: return unchecked((int)0xFFFF0000);
                case Scope.MetaPreprocessorString: return unchecked((int)0xFF1515A3);
                case Scope.MetaPreprocessorNumeric: return unchecked((int)0xFF588609);
                case Scope.MetaStructureDictionaryKeyPython: return unchecked((int)0xFFA55104);
                case Scope.Storage: return unchecked((int)0xFFFF0000);
                case Scope.StorageType: return unchecked((int)0xFFFF0000);
                case Scope.StorageModifier: return unchecked((int)0xFFFF0000);
                case Scope.KeywordOperatorNoexcept: return unchecked((int)0xFFFF0000);
                case Scope.String: return unchecked((int)0xFF1515A3);
                case Scope.MetaEmbeddedAssembly: return unchecked((int)0xFF1515A3);
                case Scope.StringCommentBufferedBlockPug: return unchecked((int)0xFFFF0000);
                case Scope.StringQuotedPug: return unchecked((int)0xFFFF0000);
                case Scope.StringInterpolatedPug: return unchecked((int)0xFFFF0000);
                case Scope.StringUnquotedPlainInYaml: return unchecked((int)0xFFFF0000);
                case Scope.StringUnquotedPlainOutYaml: return unchecked((int)0xFFFF0000);
                case Scope.StringUnquotedBlockYaml: return unchecked((int)0xFFFF0000);
                case Scope.StringQuotedSingleYaml: return unchecked((int)0xFFFF0000);
                case Scope.StringQuotedDoubleXml: return unchecked((int)0xFFFF0000);
                case Scope.StringQuotedSingleXml: return unchecked((int)0xFFFF0000);
                case Scope.StringUnquotedCdataXml: return unchecked((int)0xFFFF0000);
                case Scope.StringQuotedDoubleHtml: return unchecked((int)0xFFFF0000);
                case Scope.StringQuotedSingleHtml: return unchecked((int)0xFFFF0000);
                case Scope.StringUnquotedHtml: return unchecked((int)0xFFFF0000);
                case Scope.StringQuotedSingleHandlebars: return unchecked((int)0xFFFF0000);
                case Scope.StringQuotedDoubleHandlebars: return unchecked((int)0xFFFF0000);
                case Scope.StringRegexp: return unchecked((int)0xFF3F1F81);
                case Scope.PunctuationDefinitionTemplate_ExpressionBegin: return unchecked((int)0xFFFF0000);
                case Scope.PunctuationDefinitionTemplate_ExpressionEnd: return unchecked((int)0xFFFF0000);
                case Scope.PunctuationSectionEmbedded: return unchecked((int)0xFFFF0000);
                case Scope.MetaTemplateExpression: return unchecked((int)0xFF000000);
                case Scope.SupportConstantProperty_Value: return unchecked((int)0xFFA55104);
                case Scope.SupportConstantFont_Name: return unchecked((int)0xFFA55104);
                case Scope.SupportConstantMedia_Type: return unchecked((int)0xFFA55104);
                case Scope.SupportConstantMedia: return unchecked((int)0xFFA55104);
                case Scope.ConstantOtherColorRgb_Value: return unchecked((int)0xFFA55104);
                case Scope.ConstantOtherRgb_Value: return unchecked((int)0xFFA55104);
                case Scope.SupportConstantColor: return unchecked((int)0xFFA55104);
                case Scope.SupportTypeVendoredProperty_Name: return unchecked((int)0xFF0000E5);
                case Scope.SupportTypeProperty_Name: return unchecked((int)0xFF0000E5);
                case Scope.VariableCss: return unchecked((int)0xFF0000E5);
                case Scope.VariableScss: return unchecked((int)0xFF0000E5);
                case Scope.VariableOtherLess: return unchecked((int)0xFF0000E5);
                case Scope.SourceCoffeeEmbedded: return unchecked((int)0xFF0000E5);
                case Scope.SupportTypeProperty_NameJson: return unchecked((int)0xFFA55104);
                case Scope.Keyword: return unchecked((int)0xFFFF0000);
                case Scope.KeywordControl: return unchecked((int)0xFFDB00AF);
                case Scope.KeywordOperator: return unchecked((int)0xFF000000);
                case Scope.KeywordOperatorNew: return unchecked((int)0xFFFF0000);
                case Scope.KeywordOperatorExpression: return unchecked((int)0xFFFF0000);
                case Scope.KeywordOperatorCast: return unchecked((int)0xFFFF0000);
                case Scope.KeywordOperatorSizeof: return unchecked((int)0xFFFF0000);
                case Scope.KeywordOperatorAlignof: return unchecked((int)0xFFFF0000);
                case Scope.KeywordOperatorTypeid: return unchecked((int)0xFFFF0000);
                case Scope.KeywordOperatorAlignas: return unchecked((int)0xFFFF0000);
                case Scope.KeywordOperatorInstanceof: return unchecked((int)0xFFFF0000);
                case Scope.KeywordOperatorLogicalPython: return unchecked((int)0xFFFF0000);
                case Scope.KeywordOperatorWordlike: return unchecked((int)0xFFFF0000);
                case Scope.KeywordOtherUnit: return unchecked((int)0xFF588609);
                case Scope.PunctuationSectionEmbeddedBeginPhp: return unchecked((int)0xFF000080);
                case Scope.PunctuationSectionEmbeddedEndPhp: return unchecked((int)0xFF000080);
                case Scope.SupportFunctionGit_Rebase: return unchecked((int)0xFFA55104);
                case Scope.ConstantShaGit_Rebase: return unchecked((int)0xFF588609);
                case Scope.StorageModifierImportJava: return unchecked((int)0xFF000000);
                case Scope.VariableLanguageWildcardJava: return unchecked((int)0xFF000000);
                case Scope.StorageModifierPackageJava: return unchecked((int)0xFF000000);
                case Scope.VariableLanguage: return unchecked((int)0xFFFF0000);
                case Scope.EntityNameFunction: return unchecked((int)0xFF265E79);
                case Scope.SupportFunction: return unchecked((int)0xFF265E79);
                case Scope.SupportConstantHandlebars: return unchecked((int)0xFF265E79);
                case Scope.SourcePowershell__VariableOtherMember: return unchecked((int)0xFF265E79);
                case Scope.EntityNameOperatorCustom_Literal: return unchecked((int)0xFF265E79);
                case Scope.SupportClass: return unchecked((int)0xFF997F26);
                case Scope.SupportType: return unchecked((int)0xFF997F26);
                case Scope.EntityNameType: return unchecked((int)0xFF997F26);
                case Scope.EntityNameNamespace: return unchecked((int)0xFF997F26);
                case Scope.EntityOtherAttribute: return unchecked((int)0xFF997F26);
                case Scope.EntityNameScope_Resolution: return unchecked((int)0xFF997F26);
                case Scope.EntityNameClass: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeNumericGo: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeByteGo: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeBooleanGo: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeStringGo: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeUintptrGo: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeErrorGo: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeRuneGo: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeCs: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeGenericCs: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeModifierCs: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeVariableCs: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeAnnotationJava: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeGenericJava: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeJava: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeObjectArrayJava: return unchecked((int)0xFF997F26);
                case Scope.StorageTypePrimitiveArrayJava: return unchecked((int)0xFF997F26);
                case Scope.StorageTypePrimitiveJava: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeTokenJava: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeGroovy: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeAnnotationGroovy: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeParametersGroovy: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeGenericGroovy: return unchecked((int)0xFF997F26);
                case Scope.StorageTypeObjectArrayGroovy: return unchecked((int)0xFF997F26);
                case Scope.StorageTypePrimitiveArrayGroovy: return unchecked((int)0xFF997F26);
                case Scope.StorageTypePrimitiveGroovy: return unchecked((int)0xFF997F26);
                case Scope.MetaTypeCastExpr: return unchecked((int)0xFF997F26);
                case Scope.MetaTypeNewExpr: return unchecked((int)0xFF997F26);
                case Scope.SupportConstantMath: return unchecked((int)0xFF997F26);
                case Scope.SupportConstantDom: return unchecked((int)0xFF997F26);
                case Scope.SupportConstantJson: return unchecked((int)0xFF997F26);
                case Scope.EntityOtherInherited_Class: return unchecked((int)0xFF997F26);
                case Scope.SourceCpp__KeywordOperatorNew: return unchecked((int)0xFFDB00AF);
                case Scope.SourceCpp__KeywordOperatorDelete: return unchecked((int)0xFFDB00AF);
                case Scope.KeywordOtherUsing: return unchecked((int)0xFFDB00AF);
                case Scope.KeywordOtherDirectiveUsing: return unchecked((int)0xFFDB00AF);
                case Scope.KeywordOtherOperator: return unchecked((int)0xFFDB00AF);
                case Scope.EntityNameOperator: return unchecked((int)0xFFDB00AF);
                case Scope.Variable: return unchecked((int)0xFF801000);
                case Scope.MetaDefinitionVariableName: return unchecked((int)0xFF801000);
                case Scope.SupportVariable: return unchecked((int)0xFF801000);
                case Scope.EntityNameVariable: return unchecked((int)0xFF801000);
                case Scope.ConstantOtherPlaceholder: return unchecked((int)0xFF801000);
                case Scope.VariableOtherConstant: return unchecked((int)0xFFC17000);
                case Scope.MetaObject_LiteralKey: return unchecked((int)0xFF801000);
                case Scope.PunctuationDefinitionGroupRegexp: return unchecked((int)0xFF6969D1);
                case Scope.PunctuationDefinitionGroupAssertionRegexp: return unchecked((int)0xFF6969D1);
                case Scope.PunctuationDefinitionCharacter_ClassRegexp: return unchecked((int)0xFF6969D1);
                case Scope.PunctuationCharacterSetBeginRegexp: return unchecked((int)0xFF6969D1);
                case Scope.PunctuationCharacterSetEndRegexp: return unchecked((int)0xFF6969D1);
                case Scope.KeywordOperatorNegationRegexp: return unchecked((int)0xFF6969D1);
                case Scope.SupportOtherParenthesisRegexp: return unchecked((int)0xFF6969D1);
                case Scope.ConstantCharacterCharacter_ClassRegexp: return unchecked((int)0xFF3F1F81);
                case Scope.ConstantOtherCharacter_ClassSetRegexp: return unchecked((int)0xFF3F1F81);
                case Scope.ConstantOtherCharacter_ClassRegexp: return unchecked((int)0xFF3F1F81);
                case Scope.ConstantCharacterSetRegexp: return unchecked((int)0xFF3F1F81);
                case Scope.KeywordOperatorQuantifierRegexp: return unchecked((int)0xFF000000);
                case Scope.KeywordOperatorOrRegexp: return unchecked((int)0xFF0000EE);
                case Scope.KeywordControlAnchorRegexp: return unchecked((int)0xFF0000EE);
                case Scope.ConstantCharacter: return unchecked((int)0xFFFF0000);
                case Scope.ConstantOtherOption: return unchecked((int)0xFFFF0000);
                case Scope.ConstantCharacterEscape: return unchecked((int)0xFF0000EE);
                case Scope.EntityNameLabel: return unchecked((int)0xFF000000);
                default: return LightPlusEditorForeground;
            }
        }

        internal static int LightPlus2(Scope scope)
        {
            switch (scope)
            {
                case Scope.MetaPreprocessor: return unchecked((int)0xFF808080);
                case Scope.EntityNameTagCss: return LightPlus(Scope.EntityNameTag);
                default: return LightPlus(scope);
            }
        }
#endif
    }
}
