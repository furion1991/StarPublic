import Image from 'next/image'

import { Button, PriceWithCurrency } from '@/shared/ui'

import classes from './ContractPrizeActions.module.scss'

type ContractPrizeActionsProps = {
  prizeSellPrice: number
  onContractRetry: () => void
}

export const ContractPrizeActions = ({
  prizeSellPrice,
  onContractRetry
}: ContractPrizeActionsProps) => {
  return (
    <div className={classes.contractPrizeActions}>
      <Button onClick={onContractRetry}>
        <Image src='/icons/reload.svg' width={26} height={26} alt='Перезагрузить' /> Попробовать ещё
        раз
      </Button>

      <Button color='purple'>
        Продать за <PriceWithCurrency>{prizeSellPrice}</PriceWithCurrency>
      </Button>
    </div>
  )
}
