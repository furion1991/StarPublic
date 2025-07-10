import Image from 'next/image'
import cn from 'classnames'

import { Button, PriceWithCurrency } from '@/shared/ui'

import classes from './CaseOpenActions.module.scss'

type CaseOpenActionsProps = {
  casePrice: number
  casesQuantity: number
  isCaseOpening: boolean
  quickOpenActive: boolean
  onCaseOpen: () => void
  onCaseQuickOpen: () => void
  onCasesQuantityChange: (quantity: number) => void
}

export const CaseOpenActions = ({
  casePrice,
  casesQuantity,
  isCaseOpening,
  quickOpenActive,
  onCaseOpen,
  onCaseQuickOpen,
  onCasesQuantityChange
}: CaseOpenActionsProps) => {
  const quantityOptions = [
    { label: 'X1', value: 1 },
    { label: 'X2', value: 2 },
    { label: 'X3', value: 3 },
    { label: 'X4', value: 4 },
    { label: 'X5', value: 5 }
  ]

  return (
    <div className={classes.caseOpenActions}>
      <ul className={classes.quantitySelectorList}>
        {quantityOptions.map(({ label, value }) => {
          return (
            <li key={value}>
              <button
                type='button'
                className={cn(classes.quantityBtn, {
                  [classes.quantityBtnActive]: casesQuantity === value
                })}
                onClick={() => {
                  onCasesQuantityChange(value)
                }}
              >
                {label}
              </button>
            </li>
          )
        })}
      </ul>

      <Button
        loading={isCaseOpening}
        className={classes.openBtn}
        type='button'
        boxShadow
        onClick={onCaseOpen}
      >
        Открыть за{' '}
        <PriceWithCurrency
          image={{
            width: 25,
            height: 24
          }}
        >
          {casePrice}
        </PriceWithCurrency>
      </Button>

      <Button
        className={cn(classes.quickOpenBtn, {
          [classes.active]: quickOpenActive
        })}
        type='button'
        color='purple'
        boxShadow
        onClick={onCaseQuickOpen}
      >
        <Image src='/icons/thunder.svg' width={15} height={24} alt='Молния' />
        Быстрое открытие
      </Button>
    </div>
  )
}
