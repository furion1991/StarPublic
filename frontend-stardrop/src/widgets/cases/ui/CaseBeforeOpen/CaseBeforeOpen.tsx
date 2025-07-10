'use client'

import { CaseOpeningCondition, CasePreviewOpen } from '@/entities/cases'

import { useAuth, useUser } from '@/shared/hooks'
import { CaseOpenActions } from '@/features/cases'

import classes from './CaseBeforeOpen.module.scss'

type CaseData = {
  id: string
  openPrice: number
  name: string
  image: string
  imageType: 'FirstCategory' | 'SecondCategory'
}

type CaseBeforeOpenProps = {
  caseData: CaseData
  casesToOpenQuantity: number
  quickOpenActive: boolean
  isCaseOpening: boolean
  onCaseOpen: () => void
  onCaseQuickOpen: () => void
  onCasesQuantityChange: (quantity: number) => void
}

export const CaseBeforeOpen = ({
  caseData,
  isCaseOpening,
  quickOpenActive,
  casesToOpenQuantity,
  onCaseOpen,
  onCaseQuickOpen,
  onCasesQuantityChange
}: CaseBeforeOpenProps) => {
  const { isAuth } = useAuth()
  const { user } = useUser()

  const isBalanceNotEnough = user && user.currentBalance < caseData.openPrice ? true : false

  const getCaseOpeningCondition = (isAuth: boolean, isBalanceNotEnough: boolean) => {
    if (!isAuth) {
      return 'not-auth' as const
    }

    if (isBalanceNotEnough) {
      return 'not-enough-balance' as const
    }
  }

  const caseOpeningConidtion = getCaseOpeningCondition(isAuth, isBalanceNotEnough)

  return (
    <div className={classes.сaseBeforeOpen}>
      <CasePreviewOpen
        caseName={caseData.name}
        previewImage={caseData.image}
        previewsNumber={casesToOpenQuantity}
        imageType={caseData.imageType}
      />

      {caseOpeningConidtion ? (
        <div className={classes.caseOpeningCondition}>
          <CaseOpeningCondition
            condition={caseOpeningConidtion}
            caseOpenPrice={caseData.openPrice}
          />
        </div>
      ) : (
        <div className={classes.caseOpenActions}>
          <CaseOpenActions
            quickOpenActive={quickOpenActive}
            casesQuantity={casesToOpenQuantity}
            casePrice={caseData.openPrice * casesToOpenQuantity}
            isCaseOpening={isCaseOpening}
            onCaseOpen={onCaseOpen}
            onCaseQuickOpen={onCaseQuickOpen}
            onCasesQuantityChange={onCasesQuantityChange}
          />
        </div>
      )}
    </div>
  )
}
