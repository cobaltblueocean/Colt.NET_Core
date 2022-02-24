using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Cern.Colt.Properties;

namespace Cern
{
    using Properties = Cern.Colt.Properties;

    public class LocalizedResources : INotifyPropertyChanged
    {
        #region Singleton Class Implementation
        private readonly Resources resources = new Resources();

        private static LocalizedResources instance;

        /// <summary>
        /// Constructor implement as Singleton pattern
        /// </summary>
        private LocalizedResources()
        {
        }

        /// <summary>
        /// Return singleton instance
        /// </summary>
        /// <returns>Return current instance</returns>
        public static LocalizedResources Instance()
        {
            if (instance == null)
                instance = new LocalizedResources();

            return instance;
        }

        /// <summary>
        /// Hangling culture changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Change resource culture change
        /// </summary>
        /// <param name="name"></param>
        public void ChangeCulture(string name)
        {
            Resources.Culture = CultureInfo.GetCultureInfo(name);
            RaisePropertyChanged("Resources");
        }

        /// <summary>
        /// Get resource
        /// </summary>
        internal Resources Resources
        {
            get { return resources; }
        }

        #endregion

        public String ApplicationExcpetion_UnableToDetermineInstallRoot
        {
            get { return Properties.Resources.ApplicationExcpetion_UnableToDetermineInstallRoot; }
        }

        public String Argument_EnumIllegalVal
        {
            get { return Properties.Resources.Argument_EnumIllegalVal; }
        }

        public String Argument_InvalidEnumValue
        {
            get { return Properties.Resources.Argument_InvalidEnumValue; }
        }

        public String Argument_InvalidFlag
        {
            get { return Properties.Resources.Argument_InvalidFlag; }
        }

        public String Argument_InvalidIndexValuesString
        {
            get { return Properties.Resources.Argument_InvalidIndexValuesString; }
        }

        public String Argument_MustBeAttribute
        {
            get { return Properties.Resources.Argument_MustBeAttribute; }
        }

        public String Argument_MustBeDateTime
        {
            get { return Properties.Resources.Argument_MustBeDateTime; }
        }

        public String Argument_MustBeIsoDateTime
        {
            get { return Properties.Resources.Argument_MustBeIsoDateTime; }
        }

        public String Argument_MustBeString
        {
            get { return Properties.Resources.Argument_MustBeString; }
        }
        public String Argument_NotSerializable

        {
            get { return Properties.Resources.Argument_NotSerializable; }
        }

        public String Argument_StartIndexGreaterThanEndIndexString
        {
            get { return Properties.Resources.Argument_StartIndexGreaterThanEndIndexString; }
        }

        public String Argument_StringZeroLength
        {
            get { return Properties.Resources.Argument_StringZeroLength; }
        }

        public String ArgumentNull_String
        {
            get { return Properties.Resources.ArgumentNull_String; }
        }

        public String ArgumentOutOfRange_IndexLessThanLength
        {
            get { return Properties.Resources.ArgumentOutOfRange_IndexLessThanLength; }
        }

        public String ArgumentOutOfRange_IndexLessThanZero
        {
            get { return Properties.Resources.ArgumentOutOfRange_IndexLessThanZero; }
        }

        public String ArgumentOutOfRange_IndexString
        {
            get { return Properties.Resources.ArgumentOutOfRange_IndexString; }
        }

        public String AutoParallel_ThresholdValueNegative
        {
            get { return Properties.Resources.AutoParallel_ThresholdValueNegative; }
        }

        public String DownloadInfoConnectionClosed
        {
            get { return Properties.Resources.DownloadInfoConnectionClosed; }
        }

        public String DownloadInfoInvalidResponseReceived
        {
            get { return Properties.Resources.DownloadInfoInvalidResponseReceived; }
        }

        public String Exception_AllRowsOfArrayMustHaveSameNumberOfColumns
        {
            get { return Properties.Resources.Exception_AllRowsOfArrayMustHaveSameNumberOfColumns; }
        }

        public String Exception_ArrayLengthMustBeAMultipleOfM
        {
            get { return Properties.Resources.Exception_ArrayLengthMustBeAMultipleOfM; }
        }

        public String Exception_AssertionB_K
        {
            get { return Properties.Resources.Exception_AssertionB_K; }
        }

        public String Exception_AtLeastOneProbabilityMustBePositive
        {
            get { return Properties.Resources.Exception_AtLeastOneProbabilityMustBePositive; }
        }

        public String Exception_AttemptedToAccessAtColumn
        {
            get { return Properties.Resources.Exception_AttemptedToAccessAtColumn; }
        }

        public String Exception_AttemptedToAccessAtRow
        {
            get { return Properties.Resources.Exception_AttemptedToAccessAtRow; }
        }

        public String Exception_AttemptedToAccessAtSlice
        {
            get { return Properties.Resources.Exception_AttemptedToAccessAtSlice; }
        }

        public String Exception_BadWeight
        {
            get { return Properties.Resources.Exception_BadWeight; }
        }

        public String Exception_BothBinsMustHaveSameSize
        {
            get { return Properties.Resources.Exception_BothBinsMustHaveSameSize; }
        }
        public String Exception_BufferLengthIsZero

        {
            get { return Properties.Resources.Exception_BufferLengthIsZero; }
        }

        public String Exception_CannotStoreNonZeroValueToNonTridiagonalCoordinate
        {
            get { return Properties.Resources.Exception_CannotStoreNonZeroValueToNonTridiagonalCoordinate; }
        }

        public String Exception_CountMustNotBeGreaterThanN
        {
            get { return Properties.Resources.Exception_CountMustNotBeGreaterThanN; }
        }

        public String Exception_DataSequence
        {
            get { return Properties.Resources.Exception_DataSequence; }
        }

        public String Exception_DifferentNumberOfColumns
        {
            get { return Properties.Resources.Exception_DifferentNumberOfColumns; }
        }

        public String Exception_DifferentNumberOfRows
        {
            get { return Properties.Resources.Exception_DifferentNumberOfRows; }
        }

        public String Exception_EdgesMustBeSorted
        {
            get { return Properties.Resources.Exception_EdgesMustBeSorted; }
        }

        public String Exception_ElementIsNotContainedInDistinctElements
        {
            get { return Properties.Resources.Exception_ElementIsNotContainedInDistinctElements; }
        }

        public String Exception_ElementNotContainedInAnyBin
        {
            get { return Properties.Resources.Exception_ElementNotContainedInAnyBin; }
        }

        public String Exception_EmptyData
        {
            get { return Properties.Resources.Exception_EmptyData; }
        }

        public String Exception_FromIndexNegative
        {
            get { return Properties.Resources.Exception_FromIndexNegative; }
        }

        public String Exception_fromK_MustBeLessOrEqualTo_toK
        {
            get { return Properties.Resources.Exception_fromK_MustBeLessOrEqualTo_toK; }
        }

        public String Exception_FromToSize
        {
            get { return Properties.Resources.Exception_FromToSize; }
        }

        public String Exception_FuncionMustNotBeNull
        {
            get { return Properties.Resources.Exception_FuncionMustNotBeNull; }
        }

        public String Exception_GammaOverflow
        {
            get { return Properties.Resources.Exception_GammaOverflow; }
        }

        public String Exception_GammaSingular
        {
            get { return Properties.Resources.Exception_GammaSingular; }
        }

        public String Exception_IBetaDomainError
        {
            get { return Properties.Resources.Exception_IBetaDomainError; }
        }

        public String Exception_IllegalAxes2
        {
            get { return Properties.Resources.Exception_IllegalAxes2; }
        }

        public String Exception_IllegalAxes3
        {
            get { return Properties.Resources.Exception_IllegalAxes3; }
        }

        public String Exception_IllegalIndex
        {
            get { return Properties.Resources.Exception_IllegalIndex; }
        }

        public String Exception_IllegalInterpolationType
        {
            get { return Properties.Resources.Exception_IllegalInterpolationType; }
        }

        public String Exception_IllegalOperationOrErrorUponConstructionOf
        {
            get { return Properties.Resources.Exception_IllegalOperationOrErrorUponConstructionOf; }
        }

        public String Exception_IllegalStride
        {
            get { return Properties.Resources.Exception_IllegalStride; }
        }

        public String Exception_IllegalStrides
        {
            get { return Properties.Resources.Exception_IllegalStrides; }
        }

        public String Exception_IllegalStrides3
        {
            get { return Properties.Resources.Exception_IllegalStrides3; }
        }

        public String Exception_IlligalMissingValues
        {
            get { return Properties.Resources.Exception_IlligalMissingValues; }
        }

        public String Exception_IncompatibleArgs
        {
            get { return Properties.Resources.Exception_IncompatibleArgs; }
        }

        public String Exception_IncompatibleDimensions
        {
            get { return Properties.Resources.Exception_IncompatibleDimensions; }
        }

        public String Exception_IncompatibleDimensions2
        {
            get { return Properties.Resources.Exception_IncompatibleDimensions2; }
        }

        public String Exception_IncompatibleDimensions3
        {
            get { return Properties.Resources.Exception_IncompatibleDimensions3; }
        }

        public String Exception_IncompatibleDimensionsAandB
        {
            get { return Properties.Resources.Exception_IncompatibleDimensionsAandB; }
        }

        public String Exception_IncompatibleDimensionsAandBandC
        {
            get { return Properties.Resources.Exception_IncompatibleDimensionsAandBandC; }
        }

        public String Exception_IncompatibleResultMatrix
        {
            get { return Properties.Resources.Exception_IncompatibleResultMatrix; }
        }

        public String Exception_IncompatibleSizes
        {
            get { return Properties.Resources.Exception_IncompatibleSizes; }
        }

        public String Exception_InvalidBinRange
        {
            get { return Properties.Resources.Exception_InvalidBinRange; }
        }

        public String Exception_InvalidPermutation
        {
            get { return Properties.Resources.Exception_InvalidPermutation; }
        }

        public String Exception_KMustBePositive
        {
            get { return Properties.Resources.Exception_KMustBePositive; }
        }

        public String Exception_LagIsTooLarge
        {
            get { return Properties.Resources.Exception_LagIsTooLarge; }
        }

        public String Exception_LogGammaOverflow
        {
            get { return Properties.Resources.Exception_LogGammaOverflow; }
        }

        public String Exception_MatricesMustNotBeIdentical
        {
            get { return Properties.Resources.Exception_MatricesMustNotBeIdentical; }
        }

        public String Exception_Matrix2DInnerDimensionMustAgree
        {
            get { return Properties.Resources.Exception_Matrix2DInnerDimensionMustAgree; }
        }

        public String Exception_MatrixDimensionsMustAgree
        {
            get { return Properties.Resources.Exception_MatrixDimensionsMustAgree; }
        }
        public String Exception_MatrixIsNotSymmetricPositiveDefinite

        {
            get { return Properties.Resources.Exception_MatrixIsNotSymmetricPositiveDefinite; }
        }

        public String Exception_MatrixIsRankDeficient
        {
            get { return Properties.Resources.Exception_MatrixIsRankDeficient; }
        }

        public String Exception_MatrixIsSingular
        {
            get { return Properties.Resources.Exception_MatrixIsSingular; }
        }

        public String Exception_MatrixMustBeRectangular
        {
            get { return Properties.Resources.Exception_MatrixMustBeRectangular; }
        }

        public String Exception_MatrixMustBeSquare
        {
            get { return Properties.Resources.Exception_MatrixMustBeSquare; }
        }

        public String Exception_MatrixRowDimensionsMustAgree
        {
            get { return Properties.Resources.Exception_MatrixRowDimensionsMustAgree; }
        }

        public String Exception_MatrixTooLarge
        {
            get { return Properties.Resources.Exception_MatrixTooLarge; }
        }

        public String Exception_MustSatisfyNGraterThanOrEqualsToZero
        {
            get { return Properties.Resources.Exception_MustSatisfyNGraterThanOrEqualsToZero; }
        }

        public String Exception_NegativeCount
        {
            get { return Properties.Resources.Exception_NegativeCount; }
        }

        public String Exception_NegativeK
        {
            get { return Properties.Resources.Exception_NegativeK; }
        }

        public String Exception_NegativeProbability
        {
            get { return Properties.Resources.Exception_NegativeProbability; }
        }

        public String Exception_NegativeSize
        {
            get { return Properties.Resources.Exception_NegativeSize; }
        }

        public String Exception_NMustBeGraterThanOrEqualToZero
        {
            get { return Properties.Resources.Exception_NMustBeGraterThanOrEqualToZero; }
        }

        public String Exception_NMustBeLessThanOrEqualToN
        {
            get { return Properties.Resources.Exception_NMustBeLessThanOrEqualToN; }
        }

        public String Exception_NMustBeLessThanOrEqualToSize
        {
            get { return Properties.Resources.Exception_NMustBeLessThanOrEqualToSize; }
        }

        public String Exception_NoEmptyBuffer
        {
            get { return Properties.Resources.Exception_NoEmptyBuffer; }
        }

        public String Exception_NonExistingPdf
        {
            get { return Properties.Resources.Exception_NonExistingPdf; }
        }

        public String Exception_NonInstantiable
        {
            get { return Properties.Resources.Exception_NonInstantiable; }
        }

        public String Exception_NotEnoughData
        {
            get { return Properties.Resources.Exception_NotEnoughData; }
        }

        public String Exception_NTooLarge
        {
            get { return Properties.Resources.Exception_NTooLarge; }
        }

        public String Exception_Overflow
        {
            get { return Properties.Resources.Exception_Overflow; }
        }

        public String Exception_PartsLargerThanMatrix
        {
            get { return Properties.Resources.Exception_PartsLargerThanMatrix; }
        }

        public String Exception_PermutationsAreEnumerated
        {
            get { return Properties.Resources.Exception_PermutationsAreEnumerated; }
        }

        public String Exception_PhisMustBeAscending
        {
            get { return Properties.Resources.Exception_PhisMustBeAscending; }
        }

        public String Exception_RandomSampleExhausted
        {
            get { return Properties.Resources.Exception_RandomSampleExhausted; }
        }

        public String Exception_RankingFromTo
        {
            get { return Properties.Resources.Exception_RankingFromTo; }
        }
        public String Exception_ThisPoolInstanceIsInTheProcess

        {
            get { return Properties.Resources.Exception_ThisPoolInstanceIsInTheProcess; }
        }
        public String Exception_ThisPoolProcessHasAlreadyBeenDisposed

        {
            get { return Properties.Resources.Exception_ThisPoolProcessHasAlreadyBeenDisposed; }
        }
        public String Exception_ToIndexExceedSize

        {
            get { return Properties.Resources.Exception_ToIndexExceedSize; }
        }
        public String Exception_TooManyRows

        {
            get { return Properties.Resources.Exception_TooManyRows; }
        }
        public String Exception_ValuesTooSmall

        {
            get { return Properties.Resources.Exception_ValuesTooSmall; }
        }
        public String FileDownloaderCancelled

        {
            get { return Properties.Resources.FileDownloaderCancelled; }
        }

        public String FileDownloaderDownloadLocation
        {
            get { return Properties.Resources.FileDownloaderDownloadLocation; }
        }

        public String Format_BadDatePattern
        {
            get { return Properties.Resources.Format_BadDatePattern; }
        }

        public String Format_BadDateTime
        {
            get { return Properties.Resources.Format_BadDateTime; }
        }

        public String Format_BadDateTimeCalendar
        {
            get { return Properties.Resources.Format_BadDateTimeCalendar; }
        }

        public String Format_BadDayOfWeek
        {
            get { return Properties.Resources.Format_BadDayOfWeek; }
        }

        public String Format_BadFormatSpecifier
        {
            get { return Properties.Resources.Format_BadFormatSpecifier; }
        }

        public String Format_BadQuote
        {
            get { return Properties.Resources.Format_BadQuote; }
        }
        public String Format_DateOutOfRange

        {
            get { return Properties.Resources.Format_DateOutOfRange; }
        }

        public String Format_Dns_Bad_Ip_Address
        {
            get { return Properties.Resources.Format_Dns_Bad_Ip_Address; }
        }

        public String Format_EmptyInputString
        {
            get { return Properties.Resources.Format_EmptyInputString; }
        }

        public String Format_ExtraJunkAtEnd
        {
            get { return Properties.Resources.Format_ExtraJunkAtEnd; }
        }

        public String Format_InvalidLen
        {
            get { return Properties.Resources.Format_InvalidLen; }
        }

        public String Format_InvalidString
        {
            get { return Properties.Resources.Format_InvalidString; }
        }

        public String Format_NeedSingleChar
        {
            get { return Properties.Resources.Format_NeedSingleChar; }
        }

        public String Format_RepeatDateTimePattern
        {
            get { return Properties.Resources.Format_RepeatDateTimePattern; }
        }

        public String Format_TwoTimeZoneSpecifiers
        {
            get { return Properties.Resources.Format_TwoTimeZoneSpecifiers; }
        }

        public String Format_UnknowDateTimeWord
        {
            get { return Properties.Resources.Format_UnknowDateTimeWord; }
        }
        public String InvalidCast_FromTo
        {
            get { return Properties.Resources.InvalidCast_FromTo; }
        }

        public String Matrix_MustHaveSameNumberOfCell
        {
            get { return Properties.Resources.Matrix_MustHaveSameNumberOfCell; }
        }

        public String Matrix_MustHaveSameNumberOfColumnsInEveryRow
        {
            get { return Properties.Resources.Matrix_MustHaveSameNumberOfColumnsInEveryRow; }
        }

        public String Matrix_MustHaveSameNumberOfRows
        {
            get { return Properties.Resources.Matrix_MustHaveSameNumberOfRows; }
        }

        public String Matrix_MustHaveSameNumberOfRowsInEverySlice
        {
            get { return Properties.Resources.Matrix_MustHaveSameNumberOfRowsInEverySlice; }
        }

        public String Matrix_MustHaveSameNumberOfSlices
        {
            get { return Properties.Resources.Matrix_MustHaveSameNumberOfSlices; }
        }

        public String Matrix_VectorsMustHaveSameSize
        {
            get { return Properties.Resources.Matrix_VectorsMustHaveSameSize; }
        }

        public String MDA_InvalidFormatForLocal
        {
            get { return Properties.Resources.MDA_InvalidFormatForLocal; }
        }

        public String MDA_InvalidFormatForUtc
        {
            get { return Properties.Resources.MDA_InvalidFormatForUtc; }
        }

        public String PlatformNotSupportedMessage
        {
            get { return Properties.Resources.PlatformNotSupportedMessage; }
        }

        public String CannotCompareValues
        {
            get { return Properties.Resources.CannotCompareValues; }
        }

        public String CannotCompareProperty
        {
            get { return Properties.Resources.CannotCompareProperty; }
        }

        public String MismatchWithPropertyFound
        {
            get { return Properties.Resources.MismatchWithPropertyFound; }
        }

        public String ItemInPropertyCollectionDoesNotMatch
        {
            get { return Properties.Resources.ItemInPropertyCollectionDoesNotMatch; }
        }

        public String CollectionCountsForPropertyDoNotMatch
        {
            get { return Properties.Resources.CollectionCountsForPropertyDoNotMatch; }
        }
    }
}
