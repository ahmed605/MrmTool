// Ported from
// https://github.com/microsoft/vscode/blob/main/extensions/theme-defaults/themes/dark_plus.json
// https://github.com/microsoft/vscode/blob/main/extensions/theme-defaults/themes/dark_vs.json
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
    internal static class DarkPlusTheme
    {
        internal const int DarkPlusEditorForeground = unchecked((int)0xFFD4D4D4);

        internal static ReadOnlySpan<int> Colors
        {
            get =>
            [
                unchecked((int)0xFFD4D4D4), // MetaEmbedded
                unchecked((int)0xFFD4D4D4), // SourceGroovyEmbedded
                unchecked((int)0xFFD4D4D4), // String__MetaImageInlineMarkdown
                unchecked((int)0xFFD4D4D4), // VariableLegacyBuiltinPython
                unchecked((int)0xFFD69C56), // MetaDiffHeader
                unchecked((int)0xFF55996A), // Comment
                unchecked((int)0xFFD69C56), // ConstantLanguage
                unchecked((int)0xFFA8CEB5), // ConstantNumeric
                unchecked((int)0xFFFFC14F), // VariableOtherEnummember
                unchecked((int)0xFFA8CEB5), // KeywordOperatorPlusExponent
                unchecked((int)0xFFA8CEB5), // KeywordOperatorMinusExponent
                unchecked((int)0xFF956664), // ConstantRegexp
                unchecked((int)0xFFD69C56), // EntityNameTag
                unchecked((int)0xFFD4D4D4), // EntityNameSelector
                unchecked((int)0xFFFEDC9C), // EntityOtherAttribute_Name
                unchecked((int)0xFF7DBAD7), // EntityOtherAttribute_NameClassCss
                unchecked((int)0xFF7DBAD7), // EntityOtherAttribute_NameClassMixinCss
                unchecked((int)0xFF7DBAD7), // EntityOtherAttribute_NameIdCss
                unchecked((int)0xFF7DBAD7), // EntityOtherAttribute_NameParent_SelectorCss
                unchecked((int)0xFF7DBAD7), // EntityOtherAttribute_NamePseudo_ClassCss
                unchecked((int)0xFF7DBAD7), // EntityOtherAttribute_NamePseudo_ElementCss
                unchecked((int)0xFF7DBAD7), // SourceCssLess__EntityOtherAttribute_NameId
                unchecked((int)0xFF7DBAD7), // EntityOtherAttribute_NameScss
                unchecked((int)0xFF4747F4), // Invalid
                unchecked((int)0xFFD69C56), // MarkupBold
                unchecked((int)0xFFD69C56), // MarkupHeading
                unchecked((int)0xFFA8CEB5), // MarkupInserted
                unchecked((int)0xFF7891CE), // MarkupDeleted
                unchecked((int)0xFFD69C56), // MarkupChanged
                unchecked((int)0xFF55996A), // PunctuationDefinitionQuoteBeginMarkdown
                unchecked((int)0xFFE69667), // PunctuationDefinitionListBeginMarkdown
                unchecked((int)0xFF7891CE), // MarkupInlineRaw
                unchecked((int)0xFF808080), // PunctuationDefinitionTag
                unchecked((int)0xFF9B9B9B), // MetaPreprocessor
                unchecked((int)0xFFD69C56), // EntityNameFunctionPreprocessor
                unchecked((int)0xFF7891CE), // MetaPreprocessorString
                unchecked((int)0xFFA8CEB5), // MetaPreprocessorNumeric
                unchecked((int)0xFFFEDC9C), // MetaStructureDictionaryKeyPython
                unchecked((int)0xFFD69C56), // Storage
                unchecked((int)0xFFD69C56), // StorageType
                unchecked((int)0xFFD69C56), // StorageModifier
                unchecked((int)0xFFD69C56), // KeywordOperatorNoexcept
                unchecked((int)0xFF7891CE), // String
                unchecked((int)0xFF7891CE), // MetaEmbeddedAssembly
                unchecked((int)0xFFD4D4D4), // StringCommentBufferedBlockPug
                unchecked((int)0xFFD4D4D4), // StringQuotedPug
                unchecked((int)0xFFD4D4D4), // StringInterpolatedPug
                unchecked((int)0xFFD4D4D4), // StringUnquotedPlainInYaml
                unchecked((int)0xFFD4D4D4), // StringUnquotedPlainOutYaml
                unchecked((int)0xFFD4D4D4), // StringUnquotedBlockYaml
                unchecked((int)0xFFD4D4D4), // StringQuotedSingleYaml
                unchecked((int)0xFFD4D4D4), // StringQuotedDoubleXml
                unchecked((int)0xFFD4D4D4), // StringQuotedSingleXml
                unchecked((int)0xFFD4D4D4), // StringUnquotedCdataXml
                unchecked((int)0xFFD4D4D4), // StringQuotedDoubleHtml
                unchecked((int)0xFFD4D4D4), // StringQuotedSingleHtml
                unchecked((int)0xFFD4D4D4), // StringUnquotedHtml
                unchecked((int)0xFFD4D4D4), // StringQuotedSingleHandlebars
                unchecked((int)0xFFD4D4D4), // StringQuotedDoubleHandlebars
                unchecked((int)0xFF6969D1), // StringRegexp
                unchecked((int)0xFFD69C56), // PunctuationDefinitionTemplate_ExpressionBegin
                unchecked((int)0xFFD69C56), // PunctuationDefinitionTemplate_ExpressionEnd
                unchecked((int)0xFFD69C56), // PunctuationSectionEmbedded
                unchecked((int)0xFFD4D4D4), // MetaTemplateExpression
                unchecked((int)0xFF7891CE), // SupportConstantProperty_Value
                unchecked((int)0xFF7891CE), // SupportConstantFont_Name
                unchecked((int)0xFF7891CE), // SupportConstantMedia_Type
                unchecked((int)0xFF7891CE), // SupportConstantMedia
                unchecked((int)0xFF7891CE), // ConstantOtherColorRgb_Value
                unchecked((int)0xFF7891CE), // ConstantOtherRgb_Value
                unchecked((int)0xFF7891CE), // SupportConstantColor
                unchecked((int)0xFFFEDC9C), // SupportTypeVendoredProperty_Name
                unchecked((int)0xFFFEDC9C), // SupportTypeProperty_Name
                unchecked((int)0xFFFEDC9C), // VariableCss
                unchecked((int)0xFFFEDC9C), // VariableScss
                unchecked((int)0xFFFEDC9C), // VariableOtherLess
                unchecked((int)0xFFFEDC9C), // SourceCoffeeEmbedded
                unchecked((int)0xFFFEDC9C), // SupportTypeProperty_NameJson
                unchecked((int)0xFFD69C56), // Keyword
                unchecked((int)0xFFC086C5), // KeywordControl
                unchecked((int)0xFFD4D4D4), // KeywordOperator
                unchecked((int)0xFFD69C56), // KeywordOperatorNew
                unchecked((int)0xFFD69C56), // KeywordOperatorExpression
                unchecked((int)0xFFD69C56), // KeywordOperatorCast
                unchecked((int)0xFFD69C56), // KeywordOperatorSizeof
                unchecked((int)0xFFD69C56), // KeywordOperatorAlignof
                unchecked((int)0xFFD69C56), // KeywordOperatorTypeid
                unchecked((int)0xFFD69C56), // KeywordOperatorAlignas
                unchecked((int)0xFFD69C56), // KeywordOperatorInstanceof
                unchecked((int)0xFFD69C56), // KeywordOperatorLogicalPython
                unchecked((int)0xFFD69C56), // KeywordOperatorWordlike
                unchecked((int)0xFFA8CEB5), // KeywordOtherUnit
                unchecked((int)0xFFD69C56), // PunctuationSectionEmbeddedBeginPhp
                unchecked((int)0xFFD69C56), // PunctuationSectionEmbeddedEndPhp
                unchecked((int)0xFFFEDC9C), // SupportFunctionGit_Rebase
                unchecked((int)0xFFA8CEB5), // ConstantShaGit_Rebase
                unchecked((int)0xFFD4D4D4), // StorageModifierImportJava
                unchecked((int)0xFFD4D4D4), // VariableLanguageWildcardJava
                unchecked((int)0xFFD4D4D4), // StorageModifierPackageJava
                unchecked((int)0xFFD69C56), // VariableLanguage
                unchecked((int)0xFFAADCDC), // EntityNameFunction
                unchecked((int)0xFFAADCDC), // SupportFunction
                unchecked((int)0xFFAADCDC), // SupportConstantHandlebars
                unchecked((int)0xFFAADCDC), // SourcePowershell__VariableOtherMember
                unchecked((int)0xFFAADCDC), // EntityNameOperatorCustom_Literal
                unchecked((int)0xFFB0C94E), // SupportClass
                unchecked((int)0xFFB0C94E), // SupportType
                unchecked((int)0xFFB0C94E), // EntityNameType
                unchecked((int)0xFFB0C94E), // EntityNameNamespace
                unchecked((int)0xFFB0C94E), // EntityOtherAttribute
                unchecked((int)0xFFB0C94E), // EntityNameScope_Resolution
                unchecked((int)0xFFB0C94E), // EntityNameClass
                unchecked((int)0xFFB0C94E), // StorageTypeNumericGo
                unchecked((int)0xFFB0C94E), // StorageTypeByteGo
                unchecked((int)0xFFB0C94E), // StorageTypeBooleanGo
                unchecked((int)0xFFB0C94E), // StorageTypeStringGo
                unchecked((int)0xFFB0C94E), // StorageTypeUintptrGo
                unchecked((int)0xFFB0C94E), // StorageTypeErrorGo
                unchecked((int)0xFFB0C94E), // StorageTypeRuneGo
                unchecked((int)0xFFB0C94E), // StorageTypeCs
                unchecked((int)0xFFB0C94E), // StorageTypeGenericCs
                unchecked((int)0xFFB0C94E), // StorageTypeModifierCs
                unchecked((int)0xFFB0C94E), // StorageTypeVariableCs
                unchecked((int)0xFFB0C94E), // StorageTypeAnnotationJava
                unchecked((int)0xFFB0C94E), // StorageTypeGenericJava
                unchecked((int)0xFFB0C94E), // StorageTypeJava
                unchecked((int)0xFFB0C94E), // StorageTypeObjectArrayJava
                unchecked((int)0xFFB0C94E), // StorageTypePrimitiveArrayJava
                unchecked((int)0xFFB0C94E), // StorageTypePrimitiveJava
                unchecked((int)0xFFB0C94E), // StorageTypeTokenJava
                unchecked((int)0xFFB0C94E), // StorageTypeGroovy
                unchecked((int)0xFFB0C94E), // StorageTypeAnnotationGroovy
                unchecked((int)0xFFB0C94E), // StorageTypeParametersGroovy
                unchecked((int)0xFFB0C94E), // StorageTypeGenericGroovy
                unchecked((int)0xFFB0C94E), // StorageTypeObjectArrayGroovy
                unchecked((int)0xFFB0C94E), // StorageTypePrimitiveArrayGroovy
                unchecked((int)0xFFB0C94E), // StorageTypePrimitiveGroovy
                unchecked((int)0xFFB0C94E), // MetaTypeCastExpr
                unchecked((int)0xFFB0C94E), // MetaTypeNewExpr
                unchecked((int)0xFFB0C94E), // SupportConstantMath
                unchecked((int)0xFFB0C94E), // SupportConstantDom
                unchecked((int)0xFFB0C94E), // SupportConstantJson
                unchecked((int)0xFFB0C94E), // EntityOtherInherited_Class
                unchecked((int)0xFFC086C5), // SourceCpp__KeywordOperatorNew
                unchecked((int)0xFFD4D4D4), // SourceCpp__KeywordOperatorDelete
                unchecked((int)0xFFC086C5), // KeywordOtherUsing
                unchecked((int)0xFFC086C5), // KeywordOtherDirectiveUsing
                unchecked((int)0xFFC086C5), // KeywordOtherOperator
                unchecked((int)0xFFC086C5), // EntityNameOperator
                unchecked((int)0xFFFEDC9C), // Variable
                unchecked((int)0xFFFEDC9C), // MetaDefinitionVariableName
                unchecked((int)0xFFFEDC9C), // SupportVariable
                unchecked((int)0xFFFEDC9C), // EntityNameVariable
                unchecked((int)0xFFFEDC9C), // ConstantOtherPlaceholder
                unchecked((int)0xFFFFC14F), // VariableOtherConstant
                unchecked((int)0xFFFEDC9C), // MetaObject_LiteralKey
                unchecked((int)0xFF7891CE), // PunctuationDefinitionGroupRegexp
                unchecked((int)0xFF7891CE), // PunctuationDefinitionGroupAssertionRegexp
                unchecked((int)0xFF7891CE), // PunctuationDefinitionCharacter_ClassRegexp
                unchecked((int)0xFF7891CE), // PunctuationCharacterSetBeginRegexp
                unchecked((int)0xFF7891CE), // PunctuationCharacterSetEndRegexp
                unchecked((int)0xFF7891CE), // KeywordOperatorNegationRegexp
                unchecked((int)0xFF7891CE), // SupportOtherParenthesisRegexp
                unchecked((int)0xFF6969D1), // ConstantCharacterCharacter_ClassRegexp
                unchecked((int)0xFF6969D1), // ConstantOtherCharacter_ClassSetRegexp
                unchecked((int)0xFF6969D1), // ConstantOtherCharacter_ClassRegexp
                unchecked((int)0xFF6969D1), // ConstantCharacterSetRegexp
                unchecked((int)0xFF7DBAD7), // KeywordOperatorQuantifierRegexp
                unchecked((int)0xFFAADCDC), // KeywordOperatorOrRegexp
                unchecked((int)0xFFAADCDC), // KeywordControlAnchorRegexp
                unchecked((int)0xFFD69C56), // ConstantCharacter
                unchecked((int)0xFFD69C56), // ConstantOtherOption
                unchecked((int)0xFF7DBAD7), // ConstantCharacterEscape
                unchecked((int)0xFFC8C8C8), // EntityNameLabel
                unchecked((int)0xFF800000), // Header
                unchecked((int)0xFF7DBAD7), // EntityNameTagCss
                unchecked((int)0xFF7891CE), // StringTag
                unchecked((int)0xFF7891CE), // StringValue
                unchecked((int)0xFFC086C5), // KeywordOperatorDelete
            ];
        }

#if ENABLE_SLOW_THEME_COLOR_GETTERS
        internal static int DarkPlus(Scope scope)
        {
            switch (scope)
            {
                case Scope.MetaEmbedded: return unchecked((int)0xFFD4D4D4);
                case Scope.SourceGroovyEmbedded: return unchecked((int)0xFFD4D4D4);
                case Scope.String__MetaImageInlineMarkdown: return unchecked((int)0xFFD4D4D4);
                case Scope.VariableLegacyBuiltinPython: return unchecked((int)0xFFD4D4D4);
                case Scope.Header: return unchecked((int)0xFF800000);
                case Scope.Comment: return unchecked((int)0xFF55996A);
                case Scope.ConstantLanguage: return unchecked((int)0xFFD69C56);
                case Scope.ConstantNumeric: return unchecked((int)0xFFA8CEB5);
                case Scope.VariableOtherEnummember: return unchecked((int)0xFFFFC14F);
                case Scope.KeywordOperatorPlusExponent: return unchecked((int)0xFFA8CEB5);
                case Scope.KeywordOperatorMinusExponent: return unchecked((int)0xFFA8CEB5);
                case Scope.ConstantRegexp: return unchecked((int)0xFF956664);
                case Scope.EntityNameTag: return unchecked((int)0xFFD69C56);
                case Scope.EntityNameTagCss: return unchecked((int)0xFF7DBAD7);
                case Scope.EntityOtherAttribute_Name: return unchecked((int)0xFFFEDC9C);
                case Scope.EntityOtherAttribute_NameClassCss: return unchecked((int)0xFF7DBAD7);
                case Scope.EntityOtherAttribute_NameClassMixinCss: return unchecked((int)0xFF7DBAD7);
                case Scope.EntityOtherAttribute_NameIdCss: return unchecked((int)0xFF7DBAD7);
                case Scope.EntityOtherAttribute_NameParent_SelectorCss: return unchecked((int)0xFF7DBAD7);
                case Scope.EntityOtherAttribute_NamePseudo_ClassCss: return unchecked((int)0xFF7DBAD7);
                case Scope.EntityOtherAttribute_NamePseudo_ElementCss: return unchecked((int)0xFF7DBAD7);
                case Scope.SourceCssLess__EntityOtherAttribute_NameId: return unchecked((int)0xFF7DBAD7);
                case Scope.EntityOtherAttribute_NameScss: return unchecked((int)0xFF7DBAD7);
                case Scope.Invalid: return unchecked((int)0xFF4747F4);
                case Scope.MarkupBold: return unchecked((int)0xFFD69C56);
                case Scope.MarkupHeading: return unchecked((int)0xFFD69C56);
                case Scope.MarkupInserted: return unchecked((int)0xFFA8CEB5);
                case Scope.MarkupDeleted: return unchecked((int)0xFF7891CE);
                case Scope.MarkupChanged: return unchecked((int)0xFFD69C56);
                case Scope.PunctuationDefinitionQuoteBeginMarkdown: return unchecked((int)0xFF55996A);
                case Scope.PunctuationDefinitionListBeginMarkdown: return unchecked((int)0xFFE69667);
                case Scope.MarkupInlineRaw: return unchecked((int)0xFF7891CE);
                case Scope.PunctuationDefinitionTag: return unchecked((int)0xFF808080);
                case Scope.MetaPreprocessor: return unchecked((int)0xFFD69C56);
                case Scope.EntityNameFunctionPreprocessor: return unchecked((int)0xFFD69C56);
                case Scope.MetaPreprocessorString: return unchecked((int)0xFF7891CE);
                case Scope.MetaPreprocessorNumeric: return unchecked((int)0xFFA8CEB5);
                case Scope.MetaStructureDictionaryKeyPython: return unchecked((int)0xFFFEDC9C);
                case Scope.MetaDiffHeader: return unchecked((int)0xFFD69C56);
                case Scope.Storage: return unchecked((int)0xFFD69C56);
                case Scope.StorageType: return unchecked((int)0xFFD69C56);
                case Scope.StorageModifier: return unchecked((int)0xFFD69C56);
                case Scope.KeywordOperatorNoexcept: return unchecked((int)0xFFD69C56);
                case Scope.String: return unchecked((int)0xFF7891CE);
                case Scope.MetaEmbeddedAssembly: return unchecked((int)0xFF7891CE);
                case Scope.StringTag: return unchecked((int)0xFF7891CE);
                case Scope.StringValue: return unchecked((int)0xFF7891CE);
                case Scope.StringRegexp: return unchecked((int)0xFF6969D1);
                case Scope.PunctuationDefinitionTemplate_ExpressionBegin: return unchecked((int)0xFFD69C56);
                case Scope.PunctuationDefinitionTemplate_ExpressionEnd: return unchecked((int)0xFFD69C56);
                case Scope.PunctuationSectionEmbedded: return unchecked((int)0xFFD69C56);
                case Scope.MetaTemplateExpression: return unchecked((int)0xFFD4D4D4);
                case Scope.SupportTypeVendoredProperty_Name: return unchecked((int)0xFFFEDC9C);
                case Scope.SupportTypeProperty_Name: return unchecked((int)0xFFFEDC9C);
                case Scope.VariableCss: return unchecked((int)0xFFFEDC9C);
                case Scope.VariableScss: return unchecked((int)0xFFFEDC9C);
                case Scope.VariableOtherLess: return unchecked((int)0xFFFEDC9C);
                case Scope.SourceCoffeeEmbedded: return unchecked((int)0xFFFEDC9C);
                case Scope.Keyword: return unchecked((int)0xFFD69C56);
                case Scope.KeywordControl: return unchecked((int)0xFFC086C5);
                case Scope.KeywordOperator: return unchecked((int)0xFFD4D4D4);
                case Scope.KeywordOperatorNew: return unchecked((int)0xFFD69C56);
                case Scope.KeywordOperatorExpression: return unchecked((int)0xFFD69C56);
                case Scope.KeywordOperatorCast: return unchecked((int)0xFFD69C56);
                case Scope.KeywordOperatorSizeof: return unchecked((int)0xFFD69C56);
                case Scope.KeywordOperatorAlignof: return unchecked((int)0xFFD69C56);
                case Scope.KeywordOperatorTypeid: return unchecked((int)0xFFD69C56);
                case Scope.KeywordOperatorAlignas: return unchecked((int)0xFFD69C56);
                case Scope.KeywordOperatorInstanceof: return unchecked((int)0xFFD69C56);
                case Scope.KeywordOperatorLogicalPython: return unchecked((int)0xFFD69C56);
                case Scope.KeywordOperatorWordlike: return unchecked((int)0xFFD69C56);
                case Scope.KeywordOtherUnit: return unchecked((int)0xFFA8CEB5);
                case Scope.PunctuationSectionEmbeddedBeginPhp: return unchecked((int)0xFFD69C56);
                case Scope.PunctuationSectionEmbeddedEndPhp: return unchecked((int)0xFFD69C56);
                case Scope.SupportFunctionGit_Rebase: return unchecked((int)0xFFFEDC9C);
                case Scope.ConstantShaGit_Rebase: return unchecked((int)0xFFA8CEB5);
                case Scope.StorageModifierImportJava: return unchecked((int)0xFFD4D4D4);
                case Scope.VariableLanguageWildcardJava: return unchecked((int)0xFFD4D4D4);
                case Scope.StorageModifierPackageJava: return unchecked((int)0xFFD4D4D4);
                case Scope.VariableLanguage: return unchecked((int)0xFFD69C56);
                case Scope.EntityNameFunction: return unchecked((int)0xFFAADCDC);
                case Scope.SupportFunction: return unchecked((int)0xFFAADCDC);
                case Scope.SupportConstantHandlebars: return unchecked((int)0xFFAADCDC);
                case Scope.SourcePowershell__VariableOtherMember: return unchecked((int)0xFFAADCDC);
                case Scope.EntityNameOperatorCustom_Literal: return unchecked((int)0xFFAADCDC);
                case Scope.SupportClass: return unchecked((int)0xFFB0C94E);
                case Scope.SupportType: return unchecked((int)0xFFB0C94E);
                case Scope.EntityNameType: return unchecked((int)0xFFB0C94E);
                case Scope.EntityNameNamespace: return unchecked((int)0xFFB0C94E);
                case Scope.EntityOtherAttribute: return unchecked((int)0xFFB0C94E);
                case Scope.EntityNameScope_Resolution: return unchecked((int)0xFFB0C94E);
                case Scope.EntityNameClass: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeNumericGo: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeByteGo: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeBooleanGo: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeStringGo: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeUintptrGo: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeErrorGo: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeRuneGo: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeCs: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeGenericCs: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeModifierCs: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeVariableCs: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeAnnotationJava: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeGenericJava: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeJava: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeObjectArrayJava: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypePrimitiveArrayJava: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypePrimitiveJava: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeTokenJava: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeGroovy: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeAnnotationGroovy: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeParametersGroovy: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeGenericGroovy: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypeObjectArrayGroovy: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypePrimitiveArrayGroovy: return unchecked((int)0xFFB0C94E);
                case Scope.StorageTypePrimitiveGroovy: return unchecked((int)0xFFB0C94E);
                case Scope.MetaTypeCastExpr: return unchecked((int)0xFFB0C94E);
                case Scope.MetaTypeNewExpr: return unchecked((int)0xFFB0C94E);
                case Scope.SupportConstantMath: return unchecked((int)0xFFB0C94E);
                case Scope.SupportConstantDom: return unchecked((int)0xFFB0C94E);
                case Scope.SupportConstantJson: return unchecked((int)0xFFB0C94E);
                case Scope.EntityOtherInherited_Class: return unchecked((int)0xFFB0C94E);
                case Scope.SourceCpp__KeywordOperatorNew: return unchecked((int)0xFFC086C5);
                case Scope.KeywordOperatorDelete: return unchecked((int)0xFFC086C5);
                case Scope.KeywordOtherUsing: return unchecked((int)0xFFC086C5);
                case Scope.KeywordOtherDirectiveUsing: return unchecked((int)0xFFC086C5);
                case Scope.KeywordOtherOperator: return unchecked((int)0xFFC086C5);
                case Scope.EntityNameOperator: return unchecked((int)0xFFC086C5);
                case Scope.Variable: return unchecked((int)0xFFFEDC9C);
                case Scope.MetaDefinitionVariableName: return unchecked((int)0xFFFEDC9C);
                case Scope.SupportVariable: return unchecked((int)0xFFFEDC9C);
                case Scope.EntityNameVariable: return unchecked((int)0xFFFEDC9C);
                case Scope.ConstantOtherPlaceholder: return unchecked((int)0xFFFEDC9C);
                case Scope.VariableOtherConstant: return unchecked((int)0xFFFFC14F);
                case Scope.MetaObject_LiteralKey: return unchecked((int)0xFFFEDC9C);
                case Scope.SupportConstantProperty_Value: return unchecked((int)0xFF7891CE);
                case Scope.SupportConstantFont_Name: return unchecked((int)0xFF7891CE);
                case Scope.SupportConstantMedia_Type: return unchecked((int)0xFF7891CE);
                case Scope.SupportConstantMedia: return unchecked((int)0xFF7891CE);
                case Scope.ConstantOtherColorRgb_Value: return unchecked((int)0xFF7891CE);
                case Scope.ConstantOtherRgb_Value: return unchecked((int)0xFF7891CE);
                case Scope.SupportConstantColor: return unchecked((int)0xFF7891CE);
                case Scope.PunctuationDefinitionGroupRegexp: return unchecked((int)0xFF7891CE);
                case Scope.PunctuationDefinitionGroupAssertionRegexp: return unchecked((int)0xFF7891CE);
                case Scope.PunctuationDefinitionCharacter_ClassRegexp: return unchecked((int)0xFF7891CE);
                case Scope.PunctuationCharacterSetBeginRegexp: return unchecked((int)0xFF7891CE);
                case Scope.PunctuationCharacterSetEndRegexp: return unchecked((int)0xFF7891CE);
                case Scope.KeywordOperatorNegationRegexp: return unchecked((int)0xFF7891CE);
                case Scope.SupportOtherParenthesisRegexp: return unchecked((int)0xFF7891CE);
                case Scope.ConstantCharacterCharacter_ClassRegexp: return unchecked((int)0xFF6969D1);
                case Scope.ConstantOtherCharacter_ClassSetRegexp: return unchecked((int)0xFF6969D1);
                case Scope.ConstantOtherCharacter_ClassRegexp: return unchecked((int)0xFF6969D1);
                case Scope.ConstantCharacterSetRegexp: return unchecked((int)0xFF6969D1);
                case Scope.KeywordOperatorOrRegexp: return unchecked((int)0xFFAADCDC);
                case Scope.KeywordControlAnchorRegexp: return unchecked((int)0xFFAADCDC);
                case Scope.KeywordOperatorQuantifierRegexp: return unchecked((int)0xFF7DBAD7);
                case Scope.ConstantCharacter: return unchecked((int)0xFFD69C56);
                case Scope.ConstantOtherOption: return unchecked((int)0xFFD69C56);
                case Scope.ConstantCharacterEscape: return unchecked((int)0xFF7DBAD7);
                case Scope.EntityNameLabel: return unchecked((int)0xFFC8C8C8);
                default: return DarkPlusEditorForeground;
            }
        }

        internal static int DarkPlus2(Scope scope)
        {
            switch (scope)
            {
                case Scope.MetaPreprocessor: return unchecked((int)0xFF9B9B9B);
                case Scope.SupportTypeProperty_NameJson: return DarkPlus(Scope.SupportTypeProperty_Name);
                default: return DarkPlus(scope);
            }
        }
#endif
    }
}
