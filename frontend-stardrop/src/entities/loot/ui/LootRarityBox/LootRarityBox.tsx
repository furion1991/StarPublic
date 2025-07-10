import cn from 'classnames'
import type { CSSProperties, Ref } from 'react'

import { LootRarity } from '../../types/loot.types'

import classes from './LootRarityBox.module.scss'

type LootRarityBoxProps = {
  ref?: Ref<HTMLDivElement>
  rarity: LootRarity
  className?: string
  style?: CSSProperties
  children: React.ReactNode
}

export const LootRarityBox = ({ ref, rarity, className, style, children }: LootRarityBoxProps) => {
  return (
    <div
      ref={ref}
      className={cn(classes.lootRarityBox, className, {
        [classes.common]: rarity === LootRarity.COMMON,
        [classes.rare]: rarity === LootRarity.RARE,
        [classes.superRare]: rarity === LootRarity.SUPER_RARE,
        [classes.epic]: rarity === LootRarity.EPIC,
        [classes.mythical]: rarity === LootRarity.MYTHICAL,
        [classes.legendary]: rarity === LootRarity.LEGENDARY
      })}
      style={style}
    >
      {children}
    </div>
  )
}
