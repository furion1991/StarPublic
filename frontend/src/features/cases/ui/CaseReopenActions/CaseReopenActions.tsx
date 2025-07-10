import Image from 'next/image'

import { Button, PriceWithCurrency } from '@/shared/ui'

import classes from './CaseReopenActions.module.scss'

type CaseReopenActionsProps = {
  itemSellPrice: number
  onCaseReopen: () => void
  onItemSell: () => void
  onItemUpgrade: () => void
}

export const CaseReopenActions = ({
  itemSellPrice,
  onCaseReopen,
  onItemSell,
  onItemUpgrade
}: CaseReopenActionsProps) => {
  return (
    <div className={classes.caseReopenActions}>
      <Button color='purple' onClick={onCaseReopen}>
        <Image src='/icons/reload.svg' width={26} height={26} alt='Перезагрузить' /> Попробовать ещё
        раз
      </Button>

      <Button onClick={onItemSell}>
        Продать за <PriceWithCurrency>{itemSellPrice}</PriceWithCurrency>
      </Button>

      <Button color='cyan' onClick={onItemUpgrade}>
        <Image src='/icons/medal.svg' width={37} height={37} alt='Медаль' />
        Добавить в контракт
      </Button>
    </div>
  )
}
