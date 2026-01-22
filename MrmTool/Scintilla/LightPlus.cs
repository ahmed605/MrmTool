namespace MrmTool.Scintilla
{
    internal static class LightPlusTheme
    {
        internal const int LightPlusEditorForeground = unchecked((int)0xFF000000);

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
                default: return LightPlus(scope);
            }
        }
    }
}
