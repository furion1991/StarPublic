import Image from 'next/image'

import { PriceWithCurrency } from '@/shared/ui'

import { useBestDrop } from '../../model/useBestDrop'

import classes from './BestDroppedItem.module.scss'

type BestDroppedItemProps = {
  userId: string
}

export const BestDroppedItem = ({ userId }: BestDroppedItemProps) => {
  const { data: bestDrop, isLoading } = useBestDrop({ userId })

  if (isLoading || !bestDrop) return null

  return (
    <div className={classes.bestDroppedItem}>
      <div className={classes.top}>
        <p className={classes.title}>Лучший дроп</p>

        <div className={classes.price}>
          <PriceWithCurrency>{bestDrop.sellPrice}</PriceWithCurrency>
        </div>
      </div>

      <div className={classes.image}>
        <Image src={bestDrop.image} width={80} height={80} alt={bestDrop.name} quality={100} />
      </div>

      <p className={classes.itemName}>{bestDrop.name}</p>
    </div>
  )
}
