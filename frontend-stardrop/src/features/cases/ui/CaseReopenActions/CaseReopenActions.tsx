'use client'

import Image from 'next/image'
import { useState } from 'react'

import { Button, PriceWithCurrency } from '@/shared/ui'

import classes from './CaseReopenActions.module.scss'

type CaseReopenActionsProps = {
  itemsSellPrice: number
  onCaseReopen: () => void
  onAllItemsSell: () => void
  onItemUpgrade: () => void
}

export const CaseReopenActions = ({
  itemsSellPrice,
  onCaseReopen,
  onAllItemsSell,
  onItemUpgrade
}: CaseReopenActionsProps) => {
  const [isItemsSold, setItemsSold] = useState(false)

  return (
    <div className={classes.caseReopenActions}>
      <Button color='purple' onClick={onCaseReopen}>
        <Image src='/icons/reload.svg' width={26} height={26} alt='Перезагрузить' /> Попробовать ещё
        раз
      </Button>

      <Button
        loading={false}
        className={classes.sellBtn}
        disabled={isItemsSold}
        onClick={() => {
          onAllItemsSell()
          setItemsSold(true)
        }}
      >
        {!isItemsSold ? (
          <>
            Продать за <PriceWithCurrency>{itemsSellPrice}</PriceWithCurrency>
          </>
        ) : (
          'Продано'
        )}
      </Button>

      <Button color='cyan' onClick={onItemUpgrade}>
        <Image src='/icons/medal.svg' width={27} height={27} alt='Медаль' />
        Добавить в контракт
      </Button>
    </div>
  )
}
