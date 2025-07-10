'use client'

import { useState } from 'react'
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
  quickOpenActive: boolean
  isCaseOpening: boolean
  onCaseOpen: () => void
  onCaseQuickOpen: () => void
}

export const CaseBeforeOpen = ({
  caseData,
  isCaseOpening,
  quickOpenActive,
  onCaseOpen,
  onCaseQuickOpen
}: CaseBeforeOpenProps) => {
  const [casesToOpenQunatity, setCasesToOpenQunatity] = useState(1)

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
        hasBoxBg={casesToOpenQunatity !== 1}
        previewImage={caseData.image}
        previewsNumber={casesToOpenQunatity}
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
            casesQuantity={casesToOpenQunatity}
            casePrice={caseData.openPrice}
            isCaseOpening={isCaseOpening}
            onCaseOpen={onCaseOpen}
            onCaseQuickOpen={onCaseQuickOpen}
            onCasesQuantityChange={setCasesToOpenQunatity}
          />
        </div>
      )}
    </div>
  )
}
