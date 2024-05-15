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
    internal class LocalizedResources : INotifyPropertyChanged
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
            get { return Resources.ApplicationExcpetion_UnableToDetermineInstallRoot; }
        }

        public String Argument_EnumIllegalVal
        {
            get { return Resources.Argument_EnumIllegalVal; }
        }

        public String Argument_InvalidEnumValue
        {
            get { return Resources.Argument_InvalidEnumValue; }
        }

        public String Argument_InvalidFlag
        {
            get { return Resources.Argument_InvalidFlag; }
        }

        public String Argument_InvalidIndexValuesString
        {
            get { return Resources.Argument_InvalidIndexValuesString; }
        }

        public String Argument_MustBeAttribute
        {
            get { return Resources.Argument_MustBeAttribute; }
        }

        public String Argument_MustBeDateTime
        {
            get { return Resources.Argument_MustBeDateTime; }
        }

        public String Argument_MustBeIsoDateTime
        {
            get { return Resources.Argument_MustBeIsoDateTime; }
        }

        public String Argument_MustBeString
        {
            get { return Resources.Argument_MustBeString; }
        }
        public String Argument_NotSerializable

        {
            get { return Resources.Argument_NotSerializable; }
        }

        public String Argument_StartIndexGreaterThanEndIndexString
        {
            get { return Resources.Argument_StartIndexGreaterThanEndIndexString; }
        }

        public String Argument_StringZeroLength
        {
            get { return Resources.Argument_StringZeroLength; }
        }

        public String ArgumentNull_String
        {
            get { return Resources.ArgumentNull_String; }
        }

        public String ArgumentOutOfRange_IndexLessThanLength
        {
            get { return Resources.ArgumentOutOfRange_IndexLessThanLength; }
        }

        public String ArgumentOutOfRange_IndexLessThanZero
        {
            get { return Resources.ArgumentOutOfRange_IndexLessThanZero; }
        }

        public String ArgumentOutOfRange_IndexString
        {
            get { return Resources.ArgumentOutOfRange_IndexString; }
        }

        public String AutoParallel_ThresholdValueNegative
        {
            get { return Resources.AutoParallel_ThresholdValueNegative; }
        }

        public String DownloadInfoConnectionClosed
        {
            get { return Resources.DownloadInfoConnectionClosed; }
        }

        public String DownloadInfoInvalidResponseReceived
        {
            get { return Resources.DownloadInfoInvalidResponseReceived; }
        }

        public String Exception_AllRowsOfArrayMustHaveSameNumberOfColumns
        {
            get { return Resources.Exception_AllRowsOfArrayMustHaveSameNumberOfColumns; }
        }

        public String Exception_ArrayLengthMustBeAMultipleOfM
        {
            get { return Resources.Exception_ArrayLengthMustBeAMultipleOfM; }
        }

        public String Exception_AssertionB_K
        {
            get { return Resources.Exception_AssertionB_K; }
        }

        public String Exception_AtLeastOneProbabilityMustBePositive
        {
            get { return Resources.Exception_AtLeastOneProbabilityMustBePositive; }
        }

        public String Exception_AttemptedToAccessAtColumn
        {
            get { return Resources.Exception_AttemptedToAccessAtColumn; }
        }

        public String Exception_AttemptedToAccessAtRow
        {
            get { return Resources.Exception_AttemptedToAccessAtRow; }
        }

        public String Exception_AttemptedToAccessAtSlice
        {
            get { return Resources.Exception_AttemptedToAccessAtSlice; }
        }

        public String Exception_BadWeight
        {
            get { return Resources.Exception_BadWeight; }
        }

        public String Exception_BothBinsMustHaveSameSize
        {
            get { return Resources.Exception_BothBinsMustHaveSameSize; }
        }
        public String Exception_BufferLengthIsZero

        {
            get { return Resources.Exception_BufferLengthIsZero; }
        }

        public String Exception_CannotStoreNonZeroValueToNonTridiagonalCoordinate
        {
            get { return Resources.Exception_CannotStoreNonZeroValueToNonTridiagonalCoordinate; }
        }

        public String Exception_CountMustNotBeGreaterThanN
        {
            get { return Resources.Exception_CountMustNotBeGreaterThanN; }
        }

        public String Exception_DataSequence
        {
            get { return Resources.Exception_DataSequence; }
        }

        public String Exception_DifferentNumberOfColumns
        {
            get { return Resources.Exception_DifferentNumberOfColumns; }
        }

        public String Exception_DifferentNumberOfRows
        {
            get { return Resources.Exception_DifferentNumberOfRows; }
        }

        public String Exception_EdgesMustBeSorted
        {
            get { return Resources.Exception_EdgesMustBeSorted; }
        }

        public String Exception_ElementIsNotContainedInDistinctElements
        {
            get { return Resources.Exception_ElementIsNotContainedInDistinctElements; }
        }

        public String Exception_ElementNotContainedInAnyBin
        {
            get { return Resources.Exception_ElementNotContainedInAnyBin; }
        }

        public String Exception_EmptyData
        {
            get { return Resources.Exception_EmptyData; }
        }

        public String Exception_FromIndexNegative
        {
            get { return Resources.Exception_FromIndexNegative; }
        }

        public String Exception_fromK_MustBeLessOrEqualTo_toK
        {
            get { return Resources.Exception_fromK_MustBeLessOrEqualTo_toK; }
        }

        public String Exception_FromToSize
        {
            get { return Resources.Exception_FromToSize; }
        }

        public String Exception_FuncionMustNotBeNull
        {
            get { return Resources.Exception_FuncionMustNotBeNull; }
        }

        public String Exception_GammaOverflow
        {
            get { return Resources.Exception_GammaOverflow; }
        }

        public String Exception_GammaSingular
        {
            get { return Resources.Exception_GammaSingular; }
        }

        public String Exception_IBetaDomainError
        {
            get { return Resources.Exception_IBetaDomainError; }
        }

        public String Exception_IllegalAxes2
        {
            get { return Resources.Exception_IllegalAxes2; }
        }

        public String Exception_IllegalAxes3
        {
            get { return Resources.Exception_IllegalAxes3; }
        }

        public String Exception_IllegalIndex
        {
            get { return Resources.Exception_IllegalIndex; }
        }

        public String Exception_IllegalInterpolationType
        {
            get { return Resources.Exception_IllegalInterpolationType; }
        }

        public String Exception_IllegalOperationOrErrorUponConstructionOf
        {
            get { return Resources.Exception_IllegalOperationOrErrorUponConstructionOf; }
        }

        public String Exception_IllegalStride
        {
            get { return Resources.Exception_IllegalStride; }
        }

        public String Exception_IllegalStrides
        {
            get { return Resources.Exception_IllegalStrides; }
        }

        public String Exception_IllegalStrides3
        {
            get { return Resources.Exception_IllegalStrides3; }
        }

        public String Exception_IlligalMissingValues
        {
            get { return Resources.Exception_IlligalMissingValues; }
        }

        public String Exception_IncompatibleArgs
        {
            get { return Resources.Exception_IncompatibleArgs; }
        }

        public String Exception_IncompatibleDimensions
        {
            get { return Resources.Exception_IncompatibleDimensions; }
        }

        public String Exception_IncompatibleDimensions2
        {
            get { return Resources.Exception_IncompatibleDimensions2; }
        }

        public String Exception_IncompatibleDimensions3
        {
            get { return Resources.Exception_IncompatibleDimensions3; }
        }

        public String Exception_IncompatibleDimensionsAandB
        {
            get { return Resources.Exception_IncompatibleDimensionsAandB; }
        }

        public String Exception_IncompatibleDimensionsAandBandC
        {
            get { return Resources.Exception_IncompatibleDimensionsAandBandC; }
        }

        public String Exception_IncompatibleResultMatrix
        {
            get { return Resources.Exception_IncompatibleResultMatrix; }
        }

        public String Exception_IncompatibleSizes
        {
            get { return Resources.Exception_IncompatibleSizes; }
        }

        public String Exception_InvalidBinRange
        {
            get { return Resources.Exception_InvalidBinRange; }
        }

        public String Exception_InvalidPermutation
        {
            get { return Resources.Exception_InvalidPermutation; }
        }

        public String Exception_KMustBePositive
        {
            get { return Resources.Exception_KMustBePositive; }
        }

        public String Exception_LagIsTooLarge
        {
            get { return Resources.Exception_LagIsTooLarge; }
        }

        public String Exception_LogGammaOverflow
        {
            get { return Resources.Exception_LogGammaOverflow; }
        }

        public String Exception_MatricesMustNotBeIdentical
        {
            get { return Resources.Exception_MatricesMustNotBeIdentical; }
        }

        public String Exception_Matrix2DInnerDimensionMustAgree
        {
            get { return Resources.Exception_Matrix2DInnerDimensionMustAgree; }
        }

        public String Exception_MatrixDimensionsMustAgree
        {
            get { return Resources.Exception_MatrixDimensionsMustAgree; }
        }
        public String Exception_MatrixIsNotSymmetricPositiveDefinite

        {
            get { return Resources.Exception_MatrixIsNotSymmetricPositiveDefinite; }
        }

        public String Exception_MatrixIsRankDeficient
        {
            get { return Resources.Exception_MatrixIsRankDeficient; }
        }

        public String Exception_MatrixIsSingular
        {
            get { return Resources.Exception_MatrixIsSingular; }
        }

        public String Exception_MatrixMustBeRectangular
        {
            get { return Resources.Exception_MatrixMustBeRectangular; }
        }

        public String Exception_MatrixMustBeSquare
        {
            get { return Resources.Exception_MatrixMustBeSquare; }
        }

        public String Exception_MatrixRowDimensionsMustAgree
        {
            get { return Resources.Exception_MatrixRowDimensionsMustAgree; }
        }

        public String Exception_MatrixTooLarge
        {
            get { return Resources.Exception_MatrixTooLarge; }
        }

        public String Exception_MustSatisfyNGraterThanOrEqualsToZero
        {
            get { return Resources.Exception_MustSatisfyNGraterThanOrEqualsToZero; }
        }

        public String Exception_NegativeCount
        {
            get { return Resources.Exception_NegativeCount; }
        }

        public String Exception_NegativeK
        {
            get { return Resources.Exception_NegativeK; }
        }

        public String Exception_NegativeProbability
        {
            get { return Resources.Exception_NegativeProbability; }
        }

        public String Exception_NegativeSize
        {
            get { return Resources.Exception_NegativeSize; }
        }

        public String Exception_NMustBeGraterThanOrEqualToZero
        {
            get { return Resources.Exception_NMustBeGraterThanOrEqualToZero; }
        }

        public String Exception_NMustBeLessThanOrEqualToN
        {
            get { return Resources.Exception_NMustBeLessThanOrEqualToN; }
        }

        public String Exception_NMustBeLessThanOrEqualToSize
        {
            get { return Resources.Exception_NMustBeLessThanOrEqualToSize; }
        }

        public String Exception_NoEmptyBuffer
        {
            get { return Resources.Exception_NoEmptyBuffer; }
        }

        public String Exception_NonExistingPdf
        {
            get { return Resources.Exception_NonExistingPdf; }
        }

        public String Exception_NonInstantiable
        {
            get { return Resources.Exception_NonInstantiable; }
        }

        public String Exception_NotEnoughData
        {
            get { return Resources.Exception_NotEnoughData; }
        }

        public String Exception_NTooLarge
        {
            get { return Resources.Exception_NTooLarge; }
        }

        public String Exception_Overflow
        {
            get { return Resources.Exception_Overflow; }
        }

        public String Exception_PartsLargerThanMatrix
        {
            get { return Resources.Exception_PartsLargerThanMatrix; }
        }

        public String Exception_PermutationsAreEnumerated
        {
            get { return Resources.Exception_PermutationsAreEnumerated; }
        }

        public String Exception_PhisMustBeAscending
        {
            get { return Resources.Exception_PhisMustBeAscending; }
        }

        public String Exception_RandomSampleExhausted
        {
            get { return Resources.Exception_RandomSampleExhausted; }
        }

        public String Exception_RankingFromTo
        {
            get { return Resources.Exception_RankingFromTo; }
        }
        public String Exception_ThisPoolInstanceIsInTheProcess

        {
            get { return Resources.Exception_ThisPoolInstanceIsInTheProcess; }
        }
        public String Exception_ThisPoolProcessHasAlreadyBeenDisposed

        {
            get { return Resources.Exception_ThisPoolProcessHasAlreadyBeenDisposed; }
        }
        public String Exception_ToIndexExceedSize

        {
            get { return Resources.Exception_ToIndexExceedSize; }
        }
        public String Exception_TooManyRows

        {
            get { return Resources.Exception_TooManyRows; }
        }
        public String Exception_ValuesTooSmall

        {
            get { return Resources.Exception_ValuesTooSmall; }
        }
        public String FileDownloaderCancelled

        {
            get { return Resources.FileDownloaderCancelled; }
        }

        public String FileDownloaderDownloadLocation
        {
            get { return Resources.FileDownloaderDownloadLocation; }
        }

        public String Format_BadDatePattern
        {
            get { return Resources.Format_BadDatePattern; }
        }

        public String Format_BadDateTime
        {
            get { return Resources.Format_BadDateTime; }
        }

        public String Format_BadDateTimeCalendar
        {
            get { return Resources.Format_BadDateTimeCalendar; }
        }

        public String Format_BadDayOfWeek
        {
            get { return Resources.Format_BadDayOfWeek; }
        }

        public String Format_BadFormatSpecifier
        {
            get { return Resources.Format_BadFormatSpecifier; }
        }

        public String Format_BadQuote
        {
            get { return Resources.Format_BadQuote; }
        }
        public String Format_DateOutOfRange

        {
            get { return Resources.Format_DateOutOfRange; }
        }

        public String Format_Dns_Bad_Ip_Address
        {
            get { return Resources.Format_Dns_Bad_Ip_Address; }
        }

        public String Format_EmptyInputString
        {
            get { return Resources.Format_EmptyInputString; }
        }

        public String Format_ExtraJunkAtEnd
        {
            get { return Resources.Format_ExtraJunkAtEnd; }
        }

        public String Format_InvalidLen
        {
            get { return Resources.Format_InvalidLen; }
        }

        public String Format_InvalidString
        {
            get { return Resources.Format_InvalidString; }
        }

        public String Format_NeedSingleChar
        {
            get { return Resources.Format_NeedSingleChar; }
        }

        public String Format_RepeatDateTimePattern
        {
            get { return Resources.Format_RepeatDateTimePattern; }
        }

        public String Format_TwoTimeZoneSpecifiers
        {
            get { return Resources.Format_TwoTimeZoneSpecifiers; }
        }

        public String Format_UnknowDateTimeWord
        {
            get { return Resources.Format_UnknowDateTimeWord; }
        }
        public String InvalidCast_FromTo
        {
            get { return Resources.InvalidCast_FromTo; }
        }

        public String Matrix_MustHaveSameNumberOfCell
        {
            get { return Resources.Matrix_MustHaveSameNumberOfCell; }
        }

        public String Matrix_MustHaveSameNumberOfColumnsInEveryRow
        {
            get { return Resources.Matrix_MustHaveSameNumberOfColumnsInEveryRow; }
        }

        public String Matrix_MustHaveSameNumberOfRows
        {
            get { return Resources.Matrix_MustHaveSameNumberOfRows; }
        }

        public String Matrix_MustHaveSameNumberOfRowsInEverySlice
        {
            get { return Resources.Matrix_MustHaveSameNumberOfRowsInEverySlice; }
        }

        public String Matrix_MustHaveSameNumberOfSlices
        {
            get { return Resources.Matrix_MustHaveSameNumberOfSlices; }
        }

        public String Matrix_VectorsMustHaveSameSize
        {
            get { return Resources.Matrix_VectorsMustHaveSameSize; }
        }

        public String MDA_InvalidFormatForLocal
        {
            get { return Resources.MDA_InvalidFormatForLocal; }
        }

        public String MDA_InvalidFormatForUtc
        {
            get { return Resources.MDA_InvalidFormatForUtc; }
        }

        public String PlatformNotSupportedMessage
        {
            get { return Resources.PlatformNotSupportedMessage; }
        }
    }
}
