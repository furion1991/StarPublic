import Image from 'next/image'
import cn from 'classnames'

import { LootRarity, LootRarityBox } from '@/entities/loot'
import { PriceWithCurrency } from '@/shared/ui'

import classes from './LootItem.module.scss'

type LootItemProps = {
  className?: string
  rarity: LootRarity
  price?: number
  name: string
  image?: string
  slots?: {
    topLeft?: React.ReactNode
  }
}

export const LootItem = ({ className, image, rarity, price, name, slots }: LootItemProps) => {
  return (
    <LootRarityBox className={cn(classes.lootItem, className)} rarity={rarity}>
      <div className={classes.topLine}>
        {slots?.topLeft ? slots.topLeft : null}

        {price ? (
          <div className={classes.price}>
            <PriceWithCurrency>{price}</PriceWithCurrency>
          </div>
        ) : null}
      </div>

      <div className={classes.image}>
        <Image src={image ?? '/placeholders/case-loot.png'} fill alt={name} />
      </div>

      <p className={classes.name}>{name}</p>
    </LootRarityBox>
  )
}
