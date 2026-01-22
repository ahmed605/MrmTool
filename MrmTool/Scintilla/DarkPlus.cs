namespace MrmTool.Scintilla
{
    internal static class DarkPlusTheme
    {
        internal const int DarkPlusEditorForeground = unchecked((int)0xFFD4D4D4);

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
    }
}
