import Image from 'next/image'

import { Button, PriceWithCurrency } from '@/shared/ui'

import classes from './ContractPrizeActions.module.scss'
import { useState } from 'react'

type ContractPrizeActionsProps = {
  prizeSellPrice: number
  onContractRetry: () => void
  onItemSell: () => void
}

export const ContractPrizeActions = ({
  prizeSellPrice,
  onContractRetry,
  onItemSell
}: ContractPrizeActionsProps) => {
  const [isItemSold, setSoldItem] = useState(false)

  return (
    <div className={classes.contractPrizeActions}>
      <Button onClick={onContractRetry}>
        <Image src='/icons/reload.svg' width={26} height={26} alt='Перезагрузить' /> Попробовать ещё
        раз
      </Button>

      <Button
        color='purple'
        onClick={() => {
          onItemSell()
          setSoldItem(true)
        }}
      >
        {!isItemSold ? (
          <>
            Продать за <PriceWithCurrency>{prizeSellPrice}</PriceWithCurrency>
          </>
        ) : (
          'Продано'
        )}
      </Button>
    </div>
  )
}
