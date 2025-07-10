import Link from 'next/link'

import { UpgradeItem } from '@/entities/upgrades'
import { Button, NoDataPanel } from '@/shared/ui'

import classes from './UpgradesHistoryList.module.scss'

export const UpgradesHistoryList = () => {
  const upgrades = [
    { id: 0, result: 'success' as const },
    { id: 1, result: 'failure' as const },
    { id: 2, result: 'success' as const },
    { id: 3, result: 'failure' as const },
    { id: 4, result: 'success' as const },
    { id: 5, result: 'failure' as const }
  ]

  if (!upgrades.length) {
    return (
      <NoDataPanel
        title='Нет апгрейдов'
        text='Создайте свой первый апгрейд'
        action={
          <Link href='/'>
            <Button>Апгрейды ›</Button>
          </Link>
        }
      />
    )
  }

  return (
    <ul className={classes.upgradesHistoryList}>
      {upgrades.map(({ id, result }) => {
        return (
          <li key={id}>
            <UpgradeItem result={result} />
          </li>
        )
      })}
    </ul>
  )
}
