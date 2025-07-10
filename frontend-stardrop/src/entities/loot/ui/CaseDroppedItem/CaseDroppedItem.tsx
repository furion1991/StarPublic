import Image from 'next/image'
import cn from 'classnames'

import { LootRarityBox } from '../LootRarityBox/LootRarityBox'
import { Button } from '@/shared/ui'

import classes from './CaseDroppedItem.module.scss'

type DroppedItem = {
  rarity: number
  image: string
  name: string
  game: string
}

type CaseDroppedItemItemProps = {
  droppedItem: DroppedItem
  onItemSell: () => void
}

export const CaseDroppedItem = ({ droppedItem, onItemSell }: CaseDroppedItemItemProps) => {
  return (
    <LootRarityBox className={classes.caseDroppedItem} rarity={droppedItem.rarity}>
      <Image src={droppedItem.image} width={121} height={121} alt={droppedItem.name} />

      <p className={classes.name}>{droppedItem.name}</p>

      <Button className={classes.sellBtn} onClick={onItemSell}>
        Продать
      </Button>

      <div className={classes.triangles}>
        <div className={cn(classes.triangle, classes.triangleLeft)}>
          <Image src='/icons/triangle-right.svg' width={28} height={28} alt='Треугольник' />
        </div>

        <div className={cn(classes.triangle, classes.triangleRight)}>
          <Image src='/icons/triangle-right.svg' width={28} height={28} alt='Треугольник' />
        </div>
      </div>
    </LootRarityBox>
  )
}
